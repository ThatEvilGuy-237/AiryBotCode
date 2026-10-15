using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Hive
{
    public static class WatchActions
    {
        public const string Ignore = "ignore";
        public const string Answer = "answer";
        public const string Alert = "alert";
        public const string ReactPrefix = "react:";

        public static readonly IReadOnlyList<string> Plain = [Ignore, Answer, Alert];

        public static WatchAction Parse(string? text)
        {
            var value = (text ?? "").Trim();
            if (value.StartsWith(ReactPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var emoji = value[ReactPrefix.Length..].Trim();
                return emoji.Length is > 0 and <= 64 ? new WatchAction(ReactPrefix.TrimEnd(':'), emoji) : new WatchAction(Ignore, null);
            }
            var kind = value.ToLowerInvariant();
            return Plain.Contains(kind) ? new WatchAction(kind, null) : new WatchAction(Ignore, null);
        }

        public static bool IsValid(string? text)
        {
            var value = (text ?? "").Trim();
            if (value.StartsWith(ReactPrefix, StringComparison.OrdinalIgnoreCase)) return value.Length > ReactPrefix.Length && value.Length <= ReactPrefix.Length + 64;
            return Plain.Contains(value.ToLowerInvariant());
        }
    }

    public sealed record WatchAction(string Kind, string? Emoji);

    public sealed record WatchVerdict(string? Answer, bool Sure, double Confidence, string? Error, string? DecisionId)
    {
        public static WatchVerdict Failed(string error) => new(null, false, 0, error, null);
    }

    public sealed record WatchEvent(string Text, string Channel, ulong ChannelId, string Author, ulong AuthorId, string BotName, bool RepliesToBot, ulong? GuildId, ulong MessageId);

    public static class DecisionWatchRules
    {
        public static string Body(WatchEvent e) => new JsonObject
        {
            ["message"] = new JsonObject
            {
                ["text"] = e.Text,
                ["channel"] = e.Channel,
                ["bot"] = e.BotName,
                ["repliesToBot"] = e.RepliesToBot,
            },
            ["author"] = e.Author,
            ["authorId"] = e.AuthorId.ToString(CultureInfo.InvariantCulture),
            ["channelId"] = e.ChannelId.ToString(CultureInfo.InvariantCulture),
            ["guildId"] = e.GuildId?.ToString(CultureInfo.InvariantCulture),
            ["messageId"] = e.MessageId.ToString(CultureInfo.InvariantCulture),
        }.ToJsonString();

        public static WatchVerdict Read(string body, string? questionKey)
        {
            JsonObject? root;
            try
            {
                root = JsonNode.Parse(body) as JsonObject;
            }
            catch (JsonException)
            {
                return WatchVerdict.Failed("The listener answered something other than JSON.");
            }
            if (root?["answers"] is not JsonObject answers)
                return WatchVerdict.Failed(Text(root?["error"]) ?? "The listener answered without answers.");
            var id = Text(root["id"]);
            var key = string.IsNullOrWhiteSpace(questionKey) ? answers.Select(a => a.Key).FirstOrDefault() : questionKey.Trim();
            if (key is null || answers[key] is not JsonObject answer)
                return new WatchVerdict(null, false, 0, Text(root["error"]) ?? $"The decision has no answer to '{key}'.", id);
            var word = Text(answer["type"]) switch
            {
                "yesno" => answer["answer"] is JsonValue yes && yes.GetValueKind() == JsonValueKind.True ? "yes" : "no",
                "score" => Number(answer["level"]) is { } level ? ((int)level).ToString(CultureInfo.InvariantCulture) : null,
                _ => Text(answer["choice"]),
            };
            if (word is null) return new WatchVerdict(null, false, 0, $"The answer to '{key}' could not be read.", id);
            var sure = answer["sure"] is JsonValue s && s.GetValueKind() == JsonValueKind.True;
            return new WatchVerdict(word, sure, Number(answer["confidence"]) ?? 0, null, id);
        }

        public static IReadOnlyDictionary<string, string> Actions(string? actionsJson)
        {
            var actions = new Dictionary<string, string>(StringComparer.Ordinal);
            try
            {
                if (JsonNode.Parse(string.IsNullOrWhiteSpace(actionsJson) ? "{}" : actionsJson) is JsonObject map)
                    foreach (var (answer, action) in map)
                        if (Text(action) is { } text) actions[answer] = text;
            }
            catch (JsonException)
            {
            }
            return actions;
        }

        public static WatchAction ActionFor(ChannelDecisionWatch watch, WatchVerdict verdict)
        {
            if (verdict.Error is not null || verdict.Answer is null) return new WatchAction(WatchActions.Ignore, null);
            if (!verdict.Sure) return WatchActions.Parse(watch.UnsureAction);
            return Actions(watch.ActionsJson).TryGetValue(verdict.Answer, out var action) ? WatchActions.Parse(action) : new WatchAction(WatchActions.Ignore, null);
        }

        public static bool DecidesAnswering(ChannelDecisionWatch watch) =>
            Actions(watch.ActionsJson).Values.Append(watch.UnsureAction).Any(a => WatchActions.Parse(a).Kind == WatchActions.Answer);

        public static bool? Engage(IEnumerable<(ChannelDecisionWatch Watch, WatchAction Action)> outcomes)
        {
            bool? engage = null;
            foreach (var (watch, action) in outcomes)
            {
                if (action.Kind == WatchActions.Answer) return true;
                if (DecidesAnswering(watch)) engage = false;
            }
            return engage;
        }

        public static string AlertLine(ChannelDecisionWatch watch, WatchVerdict verdict, WatchEvent e, string jumpUrl) =>
            $"Decision watch **{watch.Name}**: {verdict.Answer} ({Math.Round(verdict.Confidence * 100).ToString(CultureInfo.InvariantCulture)} % sure{(verdict.Sure ? "" : ", unsure")}) for a message by {e.Author} in <#{e.ChannelId}>: {jumpUrl}";

        public static string Sign(string secret, string timestamp, string body)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return "sha256=" + Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes($"{timestamp}.{body}"))).ToLowerInvariant();
        }

        private static string? Text(JsonNode? node) =>
            node is JsonValue value && value.GetValueKind() == JsonValueKind.String ? value.GetValue<string>() : null;

        private static double? Number(JsonNode? node) =>
            node is JsonValue value && value.GetValueKind() == JsonValueKind.Number ? value.GetValue<double>() : null;
    }

    public class HiveDecisionClient
    {
        private readonly HttpClient _http;

        public HiveDecisionClient(HttpClient http)
        {
            _http = http;
        }

        public virtual async Task<WatchVerdict> DecideAsync(ChannelDecisionWatch watch, WatchEvent e, CancellationToken cancellationToken = default)
        {
            var body = DecisionWatchRules.Body(e);
            using var request = new HttpRequestMessage(HttpMethod.Post, watch.ListenerUrl)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };
            if (!string.IsNullOrWhiteSpace(watch.Secret))
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
                request.Headers.TryAddWithoutValidation("X-Hive-Timestamp", timestamp);
                request.Headers.TryAddWithoutValidation("X-Hive-Signature", DecisionWatchRules.Sign(watch.Secret, timestamp, body));
            }
            try
            {
                using var response = await _http.SendAsync(request, cancellationToken);
                var text = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return WatchVerdict.Failed($"The listener answered {(int)response.StatusCode}: {(text.Length <= 200 ? text : text[..200])}");
                return DecisionWatchRules.Read(text, watch.QuestionKey);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return WatchVerdict.Failed($"The listener could not be reached: {ex.Message}");
            }
        }
    }
}
