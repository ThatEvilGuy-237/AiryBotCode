using System.Text.RegularExpressions;
using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Hive
{
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
}
