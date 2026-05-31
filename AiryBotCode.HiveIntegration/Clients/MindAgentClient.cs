using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using AiryBotCode.HiveIntegration.Contracts;
using Microsoft.Extensions.Logging;

namespace AiryBotCode.HiveIntegration.Clients;

// HttpClient-backed implementation of IHiveAgentClient. One POST to Mind /agent.
// Sync (non-streaming) path only — Discord doesn't need progressive output here.
public sealed class MindAgentClient : IHiveAgentClient
{
    private readonly HttpClient _http;
    private readonly ILogger<MindAgentClient> _logger;

    // Named client lets DI configure timeout/headers in one place.
    public const string HttpClientName = "Hive.Mind";

    public MindAgentClient(IHttpClientFactory httpFactory, ILogger<MindAgentClient> logger)
    {
        _http   = httpFactory.CreateClient(HttpClientName);
        _logger = logger;
    }

    public async Task<HiveAgentResponse> SendAsync(HiveAgentRequest req, CancellationToken ct = default)
    {
        var url = req.MindUrl.TrimEnd('/') + "/agent";

        // Body shape must match Obsidian-Mind/src/routes/agentRoute.ts (BodySchema).
        // Camel-case via JsonSerializerDefaults.Web.
        var body = new
        {
            request = new
            {
                provider     = req.Provider,
                model        = req.Model,
                reasoning    = req.Reasoning,
                tooling      = req.Tooling,
                timeout      = new { duration = req.Timeout, instantStop = false },
                systemPrompt = req.SystemPrompt,
                message      = new { role = "user", content = req.UserMessage },
                history      = req.History,
                tools        = req.Tools,
                config       = new
                {
                    temperature = req.Temperature,
                    maxTokens   = req.MaxTokens
                }
            },
            metadata = new
            {
                sessionId = req.SessionId,
                userId    = req.UserId
            }
        };

        var sw = Stopwatch.StartNew();
        using var httpResp = await _http.PostAsJsonAsync(url, body, JsonOpts, ct);
        sw.Stop();

        if (!httpResp.IsSuccessStatusCode)
        {
            var err = await SafeReadAsync(httpResp, ct);
            _logger.LogError("[Hive] /agent {Status}: {Body}", (int)httpResp.StatusCode, err);
            throw new HttpRequestException($"Mind /agent returned {(int)httpResp.StatusCode}: {Truncate(err, 400)}");
        }

        // Mind returns { message: { content: "..." }, history: [...], usage: {...} }
        var node = await httpResp.Content.ReadFromJsonAsync<JsonNode>(JsonOpts, ct);
        var content = node?["message"]?["content"]?.GetValue<string>() ?? "";

        _logger.LogInformation("[Hive] /agent OK ({Ms} ms, {Chars} chars).", sw.ElapsedMilliseconds, content.Length);
        return new HiveAgentResponse
        {
            Content    = content,
            DurationMs = (int)sw.ElapsedMilliseconds
        };
    }

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private static async Task<string> SafeReadAsync(HttpResponseMessage r, CancellationToken ct)
    {
        try { return await r.Content.ReadAsStringAsync(ct); }
        catch { return $"<could not read body, status={(int)r.StatusCode}>"; }
    }

    private static string Truncate(string s, int max) =>
        string.IsNullOrEmpty(s) ? "" : (s.Length <= max ? s : s[..max] + "...");
}
