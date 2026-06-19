using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AiryBotCode.Api.Services
{
    /// <summary>
    /// One entry the control panel can offer in a picker — a Discord channel or role.
    /// </summary>
    public record DiscordEntity(string Id, string Name, string Guild);

    /// <summary>
    /// Looks up a bot's guild channels and roles straight from Discord's REST API
    /// using the bot token (read from <c>BotSettings</c> by the caller). This powers
    /// the control panel's channel/role pickers so an admin never has to paste a raw
    /// snowflake id.
    ///
    /// Registered as a singleton so its short-lived per-bot cache survives across
    /// requests. It never throws to the caller: any auth/network/Discord failure
    /// yields an empty list, and the UI falls back to a plain id input.
    /// </summary>
    public class DiscordGuildLookup
    {
        private const string ApiBase = "https://discord.com/api/v10";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

        // Channel types you can actually post into (text / announcement / forum).
        private static readonly HashSet<int> PostableChannelTypes = new() { 0, 5, 15 };
        private const int CategoryChannelType = 4;

        private readonly IHttpClientFactory _httpFactory;
        private readonly ConcurrentDictionary<ulong, CacheEntry> _cache = new();

        public DiscordGuildLookup(IHttpClientFactory httpFactory) => _httpFactory = httpFactory;

        private sealed class CacheEntry
        {
            public DateTime FetchedAt;
            public List<DiscordEntity> Channels = new();
            public List<DiscordEntity> Categories = new();
            public List<DiscordEntity> Roles = new();
        }

        public async Task<IReadOnlyList<DiscordEntity>> GetChannelsAsync(ulong botId, string? token)
            => (await GetAsync(botId, token)).Channels;

        public async Task<IReadOnlyList<DiscordEntity>> GetCategoriesAsync(ulong botId, string? token)
            => (await GetAsync(botId, token)).Categories;

        public async Task<IReadOnlyList<DiscordEntity>> GetRolesAsync(ulong botId, string? token)
            => (await GetAsync(botId, token)).Roles;

        private async Task<CacheEntry> GetAsync(ulong botId, string? token)
        {
            if (_cache.TryGetValue(botId, out var cached) && DateTime.UtcNow - cached.FetchedAt < CacheTtl)
                return cached;

            var fresh = new CacheEntry { FetchedAt = DateTime.UtcNow };
            if (!string.IsNullOrWhiteSpace(token))
            {
                try { await PopulateAsync(token!, fresh); }
                catch { /* swallow — empty lists make the UI fall back to a text input */ }
            }

            _cache[botId] = fresh;
            return fresh;
        }

        private async Task PopulateAsync(string token, CacheEntry into)
        {
            var http = _httpFactory.CreateClient();
            http.Timeout = TimeSpan.FromSeconds(8);
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", token);
            http.DefaultRequestHeaders.UserAgent.ParseAdd("AiryBotCode-Panel/1.0");

            var guilds = await GetJsonArrayAsync(http, $"{ApiBase}/users/@me/guilds");
            foreach (var guild in guilds)
            {
                var guildId = guild.GetProperty("id").GetString();
                if (guildId == null) continue;
                var guildName = guild.TryGetProperty("name", out var gn) ? gn.GetString() ?? "" : "";

                foreach (var ch in await GetJsonArrayAsync(http, $"{ApiBase}/guilds/{guildId}/channels"))
                {
                    var type = ch.TryGetProperty("type", out var t) && t.TryGetInt32(out var ti) ? ti : -1;
                    var id = ch.GetProperty("id").GetString();
                    var name = ch.TryGetProperty("name", out var cn) ? cn.GetString() : null;
                    if (id == null || name == null) continue;
                    if (PostableChannelTypes.Contains(type)) into.Channels.Add(new DiscordEntity(id, "#" + name, guildName));
                    else if (type == CategoryChannelType) into.Categories.Add(new DiscordEntity(id, name, guildName));
                }

                foreach (var role in await GetJsonArrayAsync(http, $"{ApiBase}/guilds/{guildId}/roles"))
                {
                    var id = role.GetProperty("id").GetString();
                    if (id == null || id == guildId) continue;   // skip @everyone (id == guild id)
                    var name = role.TryGetProperty("name", out var rn) ? rn.GetString() : null;
                    if (name != null) into.Roles.Add(new DiscordEntity(id, "@" + name, guildName));
                }
            }
        }

        private static async Task<List<JsonElement>> GetJsonArrayAsync(HttpClient http, string url)
        {
            using var resp = await http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return new List<JsonElement>();
            await using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            if (doc.RootElement.ValueKind != JsonValueKind.Array) return new List<JsonElement>();
            // Clone so the elements outlive the disposed JsonDocument.
            return doc.RootElement.EnumerateArray().Select(e => e.Clone()).ToList();
        }
    }
}
