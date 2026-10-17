using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AiryBotCode.Application.Conversation;

namespace AiryBotCode.Application.Hive
{
    public sealed record HiveAgentChoice(string Id, string Name);

    public sealed record HiveCheck(bool Reachable, bool CanRun, bool AgentFound, bool CanDecide, IReadOnlyList<HiveAgentChoice> Agents, IReadOnlyList<string> Deciders, IReadOnlyList<string> Problems);

    public interface IHiveDecider
    {
        Task<CheckVerdict> AskAsync(HiveConnection hive, CheckRequest check, CancellationToken ct = default);
    }

    public class HiveDecisionClient : IHiveDecider
    {
        private readonly HttpClient _http;

        public HiveDecisionClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<CheckVerdict> AskAsync(HiveConnection hive, CheckRequest check, CancellationToken ct = default)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, hive.DecisionsUrl)
                {
                    Content = new StringContent(JoinCheck.Body(check), Encoding.UTF8, "application/json"),
                };
                request.Headers.TryAddWithoutValidation(HiveAgentClient.AppKeyHeader, hive.AppKey);
                using var response = await _http.SendAsync(request, ct);
                var body = await response.Content.ReadAsStringAsync(ct);
                if (response.IsSuccessStatusCode) return JoinCheck.Read(body);
                return CheckVerdict.Failed(Refusal(response.StatusCode, body));
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return CheckVerdict.Failed(HiveTexts.Say("down", ("reason", ex.Message)));
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
}
