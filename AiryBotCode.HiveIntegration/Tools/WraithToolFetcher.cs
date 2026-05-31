using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace AiryBotCode.HiveIntegration.Tools;

// Pulls the live tool catalog from Wraith-Worker /tools. Cached per-process
// for a short window so we don't hit Wraith on every Discord message.
// Wraith hot-reloads its catalog when MCP servers reconnect, so cache TTL
// must be short enough to pick up changes without restart.
public sealed class WraithToolFetcher
{
    private readonly HttpClient _http;
    private readonly ILogger<WraithToolFetcher> _logger;

    public const string HttpClientName = "Hive.Wraith";

    // Cache lives 30s. Bot is busy enough that we'd hit Wraith N times per
    // conversation otherwise; long enough to amortize, short enough that a
    // restart-then-rebuild cycle becomes visible within half a minute.
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    private List<object>? _cached;
    private DateTimeOffset _cachedAt;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public WraithToolFetcher(IHttpClientFactory httpFactory, ILogger<WraithToolFetcher> logger)
    {
        _http   = httpFactory.CreateClient(HttpClientName);
        _logger = logger;
    }

    public async Task<IReadOnlyList<object>?> GetToolsAsync(string? wraithUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(wraithUrl)) return null;

        if (_cached is not null && DateTimeOffset.UtcNow - _cachedAt < CacheTtl) return _cached;

        await _gate.WaitAsync(ct);
        try
        {
            // Double-check after acquiring lock.
            if (_cached is not null && DateTimeOffset.UtcNow - _cachedAt < CacheTtl) return _cached;

            var url = wraithUrl.TrimEnd('/') + "/tools";
            JsonNode? node;
            try
            {
                node = await _http.GetFromJsonAsync<JsonNode>(url, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Hive] Wraith tool fetch failed; the agent will run tool-less this turn.");
                return null;
            }

            var arr = node as JsonArray;
            if (arr is null)
            {
                _logger.LogWarning("[Hive] Wraith /tools returned a non-array; ignoring.");
                return null;
            }

            var tools = new List<object>(arr.Count);
            foreach (var item in arr)
            {
                if (item is null) continue;
                // Pass through as opaque JSON-compatible object; Mind validates shape.
                tools.Add(JsonSerializer.Deserialize<object>(item.ToJsonString())!);
            }

            _cached   = tools;
            _cachedAt = DateTimeOffset.UtcNow;
            _logger.LogInformation("[Hive] Wraith tools refreshed: {Count}.", tools.Count);
            return tools;
        }
        finally
        {
            _gate.Release();
        }
    }

    // Force a refresh on the next call. Useful after the operator runs
    // POST /tools/rebuild on Wraith.
    public void Invalidate()
    {
        _cached = null;
    }
}
