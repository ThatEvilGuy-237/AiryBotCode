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

        // Usernames change rarely → a longer TTL than the guild cache.
        private static readonly TimeSpan UserCacheTtl = TimeSpan.FromMinutes(5);

        private readonly IHttpClientFactory _httpFactory;
        private readonly ConcurrentDictionary<ulong, CacheEntry> _cache = new();
        private readonly ConcurrentDictionary<ulong, (DateTime At, string Name)> _userCache = new();

        public DiscordGuildLookup(IHttpClientFactory httpFactory) => _httpFactory = httpFactory;

        private static readonly TimeSpan FailedTtl = TimeSpan.FromSeconds(10);
        private readonly ConcurrentDictionary<ulong, Lazy<Task<CacheEntry>>> _inflight = new();

        private sealed class CacheEntry
        {
            public DateTime FetchedAt;
            public bool Failed;
            public List<DiscordEntity> Channels = new();
            public List<DiscordEntity> Categories = new();
            public List<DiscordEntity> Roles = new();
        }

        public async Task<IReadOnlyList<DiscordEntity>> GetChannelsAsync(ulong botId, string? token)
            => (await GetAsync(botId, token)).Channels;

        public async Task<(IReadOnlyList<DiscordEntity> Channels, bool Known)> GetChannelsKnownAsync(ulong botId, string? token)
        {
            var entry = await GetAsync(botId, token);
            return (entry.Channels, !entry.Failed);
        }

        public async Task<IReadOnlyList<DiscordEntity>> GetCategoriesAsync(ulong botId, string? token)
            => (await GetAsync(botId, token)).Categories;

        public async Task<IReadOnlyList<DiscordEntity>> GetRolesAsync(ulong botId, string? token)
            => (await GetAsync(botId, token)).Roles;

        /// <summary>
        /// Resolve Discord user ids to display names (prefers global display name,
        /// falls back to the username), keyed by id-as-string. Best-effort and never
        /// throws: ids that can't be resolved are simply omitted, so a missing token
        /// or a deleted user just leaves the panel showing the raw id. Results are
        /// cached per id; only uncached ids hit Discord.
        /// </summary>
        public async Task<IReadOnlyDictionary<string, string>> GetUserNamesAsync(
            ulong botId, string? token, IEnumerable<ulong> userIds)
        {
            var result = new Dictionary<string, string>();
            var now = DateTime.UtcNow;

            var misses = new List<ulong>();
            foreach (var id in userIds.Where(i => i != 0).Distinct())
            {
                if (_userCache.TryGetValue(id, out var hit) && now - hit.At < UserCacheTtl)
                    result[id.ToString()] = hit.Name;
                else
                    misses.Add(id);
            }

            if (misses.Count == 0 || string.IsNullOrWhiteSpace(token))
                return result;

            try
            {
                var http = _httpFactory.CreateClient();
                http.Timeout = TimeSpan.FromSeconds(8);
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", token);
                http.DefaultRequestHeaders.UserAgent.ParseAdd("AiryBotCode-Panel/1.0");

                foreach (var id in misses)
                {
                    try
                    {
                        using var resp = await http.GetAsync($"{ApiBase}/users/{id}");
                        if (!resp.IsSuccessStatusCode) continue;
                        await using var stream = await resp.Content.ReadAsStreamAsync();
                        using var doc = await JsonDocument.ParseAsync(stream);
                        var root = doc.RootElement;
                        var name =
                            (root.TryGetProperty("global_name", out var gn) ? gn.GetString() : null)
                            ?? (root.TryGetProperty("username", out var un) ? un.GetString() : null);
                        if (string.IsNullOrWhiteSpace(name)) continue;
                        _userCache[id] = (now, name);
                        result[id.ToString()] = name;
                    }
                    catch { /* skip this id — best-effort */ }
                }
            }
            catch { /* swallow — partial/empty map just shows raw ids */ }

            return result;
        }

        private async Task<CacheEntry> GetAsync(ulong botId, string? token)
        {
            if (_cache.TryGetValue(botId, out var cached) && DateTime.UtcNow - cached.FetchedAt < (cached.Failed ? FailedTtl : CacheTtl))
                return cached;

            var fetch = _inflight.GetOrAdd(botId, _ => new Lazy<Task<CacheEntry>>(() => FetchAsync(botId, token)));
            try
            {
                return await fetch.Value;
            }
            finally
            {
                _inflight.TryRemove(new KeyValuePair<ulong, Lazy<Task<CacheEntry>>>(botId, fetch));
            }
        }

        private async Task<CacheEntry> FetchAsync(ulong botId, string? token)
        {
            var fresh = new CacheEntry { FetchedAt = DateTime.UtcNow, Failed = string.IsNullOrWhiteSpace(token) };
            if (!fresh.Failed)
            {
                try { fresh.Failed = !await PopulateAsync(token!, fresh); }
                catch { fresh.Failed = true; }
            }
            if (fresh.Failed && _cache.TryGetValue(botId, out var lastGood) && !lastGood.Failed)
            {
                lastGood.FetchedAt = DateTime.UtcNow - CacheTtl + FailedTtl;
                return lastGood;
            }
            _cache[botId] = fresh;
            return fresh;
        }

        private async Task<bool> PopulateAsync(string token, CacheEntry into)
        {
            var http = _httpFactory.CreateClient();
            http.Timeout = TimeSpan.FromSeconds(8);
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", token);
            http.DefaultRequestHeaders.UserAgent.ParseAdd("AiryBotCode-Panel/1.0");

            var (guilds, ok) = await GetGuildsAsync(http);
            if (!ok) return false;
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
            return true;
        }

        private static async Task<(List<JsonElement> Guilds, bool Ok)> GetGuildsAsync(HttpClient http)
        {
            for (var attempt = 0; attempt < 2; attempt++)
            {
                using var resp = await http.GetAsync($"{ApiBase}/users/@me/guilds");
                if (resp.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt == 0)
                {
                    var wait = resp.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(1);
                    if (wait > TimeSpan.FromSeconds(3)) return (new List<JsonElement>(), false);
                    await Task.Delay(wait);
                    continue;
                }
                if (!resp.IsSuccessStatusCode) return (new List<JsonElement>(), false);
                await using var stream = await resp.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                if (doc.RootElement.ValueKind != JsonValueKind.Array) return (new List<JsonElement>(), false);
                return (doc.RootElement.EnumerateArray().Select(e => e.Clone()).ToList(), true);
            }
            return (new List<JsonElement>(), false);
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
