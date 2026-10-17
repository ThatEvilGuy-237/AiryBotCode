using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using AiryBotCode.Application.Hive;

namespace AiryBotCode.Application.Conversation
{
    public sealed record CheckLine(string Author, string Text);

    public sealed record CheckRequest(string Bot, string Channel, string Author, ulong AuthorId, string Message, IReadOnlyList<CheckLine> Recent, bool FollowUp = false);

    public sealed record CheckVerdict(bool? Yes, bool Sure, double Confidence, string? DecisionId, string? Error)
    {
        public bool SaysAnswer => Error is null && Yes == true && Sure;

        public string Summary => Error is not null
            ? "failed"
            : $"{(Yes == true ? "yes" : "no")}{(Sure ? "" : ", unsure")} {Math.Round(Confidence * 100).ToString(CultureInfo.InvariantCulture)}%";

        public static CheckVerdict Failed(string error) => new(null, false, 0, null, error);
    }

    public static class JoinCheck
    {
        public const string Key = "worth";
        private const string File = "join-check";

        public static string Body(CheckRequest e)
        {
            var settings = ConversationSettings.Default;
            var recent = e.Recent.TakeLast(settings.CheckLines).Select(l => $"{l.Author}: {Texts.Flat(l.Text, settings.LineChars)}").ToList();
            var fill = new Dictionary<string, string>
            {
                ["bot"] = e.Bot,
                ["channel"] = e.Channel,
                ["author"] = e.Author,
                ["message"] = Texts.Flat(e.Message, settings.MessageChars),
                ["recent"] = recent.Count == 0 ? Texts.Section(File, "no recent") : string.Join("\n", recent),
            };
            var body = new JsonObject
            {
                ["state"] = Texts.Fill(Texts.Section(File, e.FollowUp ? "follow-up state" : "join-in state"), fill),
                ["questions"] = new JsonObject
                {
                    [Key] = new JsonObject
                    {
                        ["type"] = "yesno",
                        ["instructions"] = Texts.Fill(Texts.Section(File, e.FollowUp ? "follow-up" : "join-in"), fill),
                    },
                },
                ["user"] = $"discord:{e.AuthorId}",
            };
            return body.ToJsonString();
        }

        public static CheckVerdict Read(string json)
        {
            try
            {
                var root = JsonNode.Parse(json)?.AsObject();
                if (root is null) return CheckVerdict.Failed(HiveTexts.Say("no answer"));
                var status = root["status"]?.GetValue<string>();
                var answer = root["answers"]?[Key];
                if (answer is null)
                    return CheckVerdict.Failed(root["error"]?.GetValue<string>() ?? HiveTexts.Say("no verdict", ("status", status ?? "nothing")));
                bool? yes = answer["answer"] is JsonValue v && v.TryGetValue<bool>(out var b) ? b : null;
                var sure = answer["sure"] is JsonValue s && s.TryGetValue<bool>(out var sv) && sv;
                var confidence = answer["confidence"] is JsonValue c && c.TryGetValue<double>(out var cv) ? cv : 0;
                return yes is null
                    ? CheckVerdict.Failed(HiveTexts.Say("not yes no"))
                    : new CheckVerdict(yes, sure, confidence, root["id"]?.GetValue<string>(), null);
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException or FormatException)
            {
                return CheckVerdict.Failed(HiveTexts.Say("unreadable"));
            }
        }
    }
}
