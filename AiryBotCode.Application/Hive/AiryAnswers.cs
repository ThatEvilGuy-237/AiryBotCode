using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Hive
{
    public static class AnswerModes
    {
        public const string Always = "always";
        public const string Mention = "mention";
        public const string Decide = "decide";

        public static readonly IReadOnlyList<string> All = [Always, Mention, Decide];

        public static bool IsValid(string? mode) => mode is not null && All.Contains(mode);
    }

    public sealed record HiveConnection(string Url, string AgentId, string AppKey)
    {
        public string RunUrl => $"{Url}/agents/{AgentId}/run";
        public string DecisionsUrl => $"{Url}/v1/decisions";
        public string DecidersUrl => $"{Url}/v1/deciders";
        public string AgentsUrl => $"{Url}/v1/agents";

        private static readonly Regex RunPath = new(@"^(?<url>https?://.+?)/agents/(?<agent>[0-9a-fA-F-]{36})/run/?$", RegexOptions.Compiled);

        public static HiveConnection? Of(BotSetting bot) =>
            string.IsNullOrWhiteSpace(bot.HiveUrl) || string.IsNullOrWhiteSpace(bot.HiveAgentId) || string.IsNullOrWhiteSpace(bot.HiveAppKey)
                ? null
                : new HiveConnection(NormalUrl(bot.HiveUrl), bot.HiveAgentId.Trim(), bot.HiveAppKey.Trim());

        public static HiveConnection? FromRunUrl(string? runUrl, string? appKey)
        {
            if (string.IsNullOrWhiteSpace(runUrl) || string.IsNullOrWhiteSpace(appKey)) return null;
            var match = RunPath.Match(runUrl.Trim().Split('?')[0]);
            return match.Success ? new HiveConnection(NormalUrl(match.Groups["url"].Value), match.Groups["agent"].Value.ToLowerInvariant(), appKey.Trim()) : null;
        }

        public static string NormalUrl(string url) => url.Trim().TrimEnd('/');

        public static bool IsHiveUrl(string? url) =>
            Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https";
    }

    public sealed record WorthVerdict(bool? Yes, bool Sure, double Confidence, string? DecisionId, string? Error)
    {
        public bool SaysAnswer => Error is null && Yes == true && Sure;

        public string Summary => Error is not null
            ? "failed"
            : $"{(Yes == true ? "yes" : "no")}{(Sure ? "" : ", unsure")} {Math.Round(Confidence * 100).ToString(CultureInfo.InvariantCulture)}%";

        public static WorthVerdict Failed(string error) => new(null, false, 0, null, error);
    }

    public static class AnswerRules
    {
        public static bool Hears(bool listens, bool consented) => listens && consented;

        public static bool Watches(string mode, bool targeted, bool consented) =>
            mode == AnswerModes.Decide && !targeted && consented;

        public static bool Answers(string mode, bool targeted, WorthVerdict? verdict) => mode switch
        {
            AnswerModes.Always => true,
            AnswerModes.Decide => targeted || verdict is { SaysAnswer: true },
            _ => targeted,
        };
    }

    public sealed record RecentLine(string Author, string Text);

    public sealed record WorthEvent(string Bot, string Channel, string Author, ulong AuthorId, string Message, IReadOnlyList<RecentLine> Recent, bool FollowUp = false);

    public static class WorthQuestion
    {
        public const string Key = "worth";
        public const int MaxRecent = 12;
        public const int MaxLine = 300;

        private static readonly Lazy<IReadOnlyDictionary<string, string>> Sections = new(() => ReadSections("worth-answering"));

        public static string Body(WorthEvent e)
        {
            var recent = e.Recent.TakeLast(MaxRecent).Select(l => $"{l.Author}: {Cut(l.Text)}").ToList();
            var fill = new Dictionary<string, string>
            {
                ["bot"] = e.Bot,
                ["channel"] = e.Channel,
                ["author"] = e.Author,
                ["message"] = Cut(e.Message, 2000),
                ["recent"] = recent.Count == 0 ? Sections.Value["no recent"] : string.Join("\n", recent),
            };
            var body = new JsonObject
            {
                ["state"] = Fill(Sections.Value[e.FollowUp ? "follow-up state" : "state"], fill),
                ["questions"] = new JsonObject
                {
                    [Key] = new JsonObject
                    {
                        ["type"] = "yesno",
                        ["instructions"] = Fill(Sections.Value[e.FollowUp ? "follow-up" : "instructions"], fill),
                    },
                },
                ["user"] = $"discord:{e.AuthorId}",
            };
            return body.ToJsonString();
        }

        public static WorthVerdict Read(string json)
        {
            try
            {
                var root = JsonNode.Parse(json)?.AsObject();
                if (root is null) return WorthVerdict.Failed(HiveTexts.Say("no answer"));
                var status = root["status"]?.GetValue<string>();
                var answer = root["answers"]?[Key];
                if (answer is null)
                    return WorthVerdict.Failed(root["error"]?.GetValue<string>() ?? HiveTexts.Say("no verdict", ("status", status ?? "nothing")));
                bool? yes = answer["answer"] is JsonValue v && v.TryGetValue<bool>(out var b) ? b : null;
                var sure = answer["sure"] is JsonValue s && s.TryGetValue<bool>(out var sv) && sv;
                var confidence = answer["confidence"] is JsonValue c && c.TryGetValue<double>(out var cv) ? cv : 0;
                return yes is null
                    ? WorthVerdict.Failed(HiveTexts.Say("not yes no"))
                    : new WorthVerdict(yes, sure, confidence, root["id"]?.GetValue<string>(), null);
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException or FormatException)
            {
                return WorthVerdict.Failed(HiveTexts.Say("unreadable"));
            }
        }

        public static string Fill(string template, IReadOnlyDictionary<string, string> values) =>
            Regex.Replace(template, @"\{\{(\w+)\}\}", m => values.TryGetValue(m.Groups[1].Value, out var v) ? v : m.Value).Trim();

        private static string Cut(string text, int max = MaxLine)
        {
            var flat = text.Replace('\n', ' ').Trim();
            return flat.Length <= max ? flat : flat[..max] + "…";
        }

        public static IReadOnlyDictionary<string, string> ReadSections(string name)
        {
            using var stream = typeof(WorthQuestion).Assembly.GetManifestResourceStream($"AiryBotCode.Hive.{name}.md")
                ?? throw new InvalidOperationException($"Text {name}.md is not embedded.");
            using var reader = new StreamReader(stream);
            var sections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string? current = null;
            var buffer = new StringBuilder();
            foreach (var line in reader.ReadToEnd().Split('\n'))
            {
                if (line.StartsWith("## "))
                {
                    if (current is not null) sections[current] = buffer.ToString().Trim();
                    current = line[3..].Trim();
                    buffer.Clear();
                }
                else if (current is not null) buffer.AppendLine(line.TrimEnd('\r'));
            }
            if (current is not null) sections[current] = buffer.ToString().Trim();
            return sections;
        }
    }

    public static class HiveTexts
    {
        private static readonly Lazy<IReadOnlyDictionary<string, string>> Messages = new(() => WorthQuestion.ReadSections("hive-messages"));

        public static string Say(string key, params (string Name, string Value)[] values) =>
            WorthQuestion.Fill(Messages.Value[key], values.ToDictionary(v => v.Name, v => v.Value));
    }

    public sealed record HiveAgentChoice(string Id, string Name);

    public sealed record HiveCheck(bool Reachable, bool CanRun, bool AgentFound, bool CanDecide, IReadOnlyList<HiveAgentChoice> Agents, IReadOnlyList<string> Deciders, IReadOnlyList<string> Problems);

    public class HiveAnswerClient
    {
        private readonly HttpClient _http;

        public HiveAnswerClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<WorthVerdict> AskAsync(HiveConnection hive, WorthEvent e, CancellationToken ct = default)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, hive.DecisionsUrl)
                {
                    Content = new StringContent(WorthQuestion.Body(e), Encoding.UTF8, "application/json"),
                };
                request.Headers.TryAddWithoutValidation(HiveAgentClient.AppKeyHeader, hive.AppKey);
                using var response = await _http.SendAsync(request, ct);
                var body = await response.Content.ReadAsStringAsync(ct);
                if (response.IsSuccessStatusCode) return WorthQuestion.Read(body);
                return WorthVerdict.Failed(Refusal(response.StatusCode, body));
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return WorthVerdict.Failed(HiveTexts.Say("down", ("reason", ex.Message)));
            }
        }

        public async Task<IReadOnlyList<HiveAgentChoice>?> AgentsAsync(string url, string appKey, CancellationToken ct = default)
        {
            var (status, body) = await GetAsync($"{HiveConnection.NormalUrl(url)}/v1/agents", appKey, ct);
            if (status != HttpStatusCode.OK || body is null) return null;
            try
            {
                return (JsonNode.Parse(body) as JsonArray ?? [])
                    .Select(a => new HiveAgentChoice(a?["id"]?.GetValue<string>() ?? "", a?["name"]?.GetValue<string>() ?? ""))
                    .Where(a => a.Id.Length > 0)
                    .ToList();
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public async Task<HiveCheck> CheckAsync(string url, string appKey, string? agentId, CancellationToken ct = default)
        {
            var problems = new List<string>();
            var (agentStatus, agentBody) = await GetAsync($"{HiveConnection.NormalUrl(url)}/v1/agents", appKey, ct);
            if (agentStatus == 0)
                return new HiveCheck(false, false, false, false, [], [], [HiveTexts.Say("unreachable")]);

            var agents = new List<HiveAgentChoice>();
            var canRun = agentStatus == HttpStatusCode.OK;
            if (canRun && agentBody is not null)
            {
                try
                {
                    agents = (JsonNode.Parse(agentBody) as JsonArray ?? [])
                        .Select(a => new HiveAgentChoice(a?["id"]?.GetValue<string>() ?? "", a?["name"]?.GetValue<string>() ?? ""))
                        .Where(a => a.Id.Length > 0)
                        .ToList();
                }
                catch (JsonException) { }
            }
            else problems.Add(HiveTexts.Say(agentStatus == HttpStatusCode.Unauthorized ? "unknown key" : "cannot run"));

            var agentFound = agentId is not null && agents.Any(a => string.Equals(a.Id, agentId, StringComparison.OrdinalIgnoreCase));
            if (canRun && agentId is not null && !agentFound) problems.Add(HiveTexts.Say("agent missing"));

            var (deciderStatus, deciderBody) = await GetAsync($"{HiveConnection.NormalUrl(url)}/v1/deciders", appKey, ct);
            var canDecide = deciderStatus == HttpStatusCode.OK;
            var deciders = new List<string>();
            if (canDecide && deciderBody is not null)
            {
                try
                {
                    deciders = (JsonNode.Parse(deciderBody) as JsonArray ?? [])
                        .Select(d => d?["name"]?.GetValue<string>() ?? "")
                        .Where(n => n.Length > 0)
                        .ToList();
                }
                catch (JsonException) { }
                if (deciders.Count == 0) problems.Add(HiveTexts.Say("no decider"));
            }
            else if (deciderStatus == HttpStatusCode.NotFound) problems.Add(HiveTexts.Say("decisions off"));
            else if (deciderStatus != HttpStatusCode.Unauthorized) problems.Add(HiveTexts.Say("cannot decide"));

            return new HiveCheck(true, canRun, agentFound, canDecide, agents, deciders, problems);
        }

        private async Task<(HttpStatusCode Status, string? Body)> GetAsync(string url, string appKey, CancellationToken ct)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.TryAddWithoutValidation(HiveAgentClient.AppKeyHeader, appKey);
                using var response = await _http.SendAsync(request, ct);
                return (response.StatusCode, await response.Content.ReadAsStringAsync(ct));
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or InvalidOperationException)
            {
                return (0, null);
            }
        }

        public static string Refusal(HttpStatusCode status, string body)
        {
            string? said = null;
            try { said = JsonNode.Parse(body)?["error"]?.GetValue<string>(); } catch (Exception) { }
            return status switch
            {
                HttpStatusCode.Unauthorized => HiveTexts.Say("refused key"),
                HttpStatusCode.Forbidden => HiveTexts.Say("refused decide"),
                HttpStatusCode.NotFound => said ?? HiveTexts.Say("refused off"),
                _ => said ?? HiveTexts.Say("refused other", ("status", ((int)status).ToString(CultureInfo.InvariantCulture))),
            };
        }
    }

    public sealed record AgentLink(int Id, ulong ChannelId, string RunUrl, string? Key, bool Enabled);

    public sealed record ChannelPlan(ulong ChannelId, string Mode, bool Enabled);

    public sealed record LinkMove(HiveConnection? Connection, IReadOnlyList<ChannelPlan> Channels, IReadOnlyList<int> RemoveLinks)
    {
        public bool IsEmpty => Connection is null && Channels.Count == 0 && RemoveLinks.Count == 0;
    }

    public static class AgentLinkMove
    {
        public static LinkMove Plan(HiveConnection? current, IReadOnlyCollection<ulong> existingChannels, IReadOnlyList<AgentLink> links, bool requireMention)
        {
            var target = current ?? links
                .OrderByDescending(l => l.Enabled)
                .ThenBy(l => l.Id)
                .Select(l => HiveConnection.FromRunUrl(l.RunUrl, l.Key))
                .FirstOrDefault(c => c is not null);
            if (target is null) return new LinkMove(null, [], []);

            var channels = new List<ChannelPlan>();
            var removed = new List<int>();
            var taken = new HashSet<ulong>(existingChannels);
            foreach (var link in links.OrderBy(l => l.Id))
            {
                if (HiveConnection.FromRunUrl(link.RunUrl, link.Key) != target) continue;
                removed.Add(link.Id);
                if (taken.Add(link.ChannelId))
                    channels.Add(new ChannelPlan(link.ChannelId, requireMention ? AnswerModes.Mention : AnswerModes.Always, link.Enabled));
            }
            return removed.Count == 0
                ? new LinkMove(null, [], [])
                : new LinkMove(current is null ? target : null, channels, removed);
        }
    }
}
