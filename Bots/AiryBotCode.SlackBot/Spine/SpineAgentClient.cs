using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AiryBotCode.SlackBot.Spine;

/// <summary>
/// Calls a saved Neural-Spine agent (Vinnie) synchronously and returns its reply.
/// Mirrors AiryBotCode's shared JWT (HS256, issuer/audience = AiryBotCode) so the
/// same secret authenticates against Spine's FallbackPolicy. The agent run does
/// Chronos memory recall/ingest itself, keyed by the sessionId/userId we pass.
/// </summary>
public sealed class SpineAgentClient
{
    private readonly HttpClient _http;
    private readonly string _spineUrl;   // e.g. http://spine:7000
    private readonly string _agentId;    // Vinnie agent id
    private readonly string _secret;     // shared AiryBotCode JWT secret

    public SpineAgentClient(HttpClient http, string spineUrl, string agentId, string secret)
    {
        _http = http;
        _spineUrl = spineUrl.TrimEnd('/');
        _agentId = agentId;
        _secret = secret;
    }

    public async Task<string> AskAsync(string message, string sessionId, string userId, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_spineUrl}/agents/{_agentId}/run");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", MintJwt(_secret));
        var body = JsonSerializer.Serialize(new { message, sessionId, userId });
        req.Content = new StringContent(body, Encoding.UTF8, "application/json");

        using var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();
        await using var stream = await res.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        if (doc.RootElement.TryGetProperty("text", out var text) && text.ValueKind == JsonValueKind.String)
            return text.GetString() ?? string.Empty;
        return string.Empty;
    }

    // Short-lived HS256 token Spine accepts (issuer/audience = AiryBotCode), mirroring
    // AiryBotCode's scripts/uitest/mint-jwt.cjs. Cheap enough to mint per request.
    private static string MintJwt(string secret)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var header = B64Url(JsonSerializer.SerializeToUtf8Bytes(new { alg = "HS256", typ = "JWT" }));
        var claims = new Dictionary<string, object>
        {
            ["sub"] = "vinnie-slack",
            ["nameid"] = "vinnie-slack",
            ["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] = "vinnie-slack",
            ["iss"] = "AiryBotCode",
            ["aud"] = "AiryBotCode",
            ["iat"] = now,
            ["exp"] = now + 300,
        };
        var payload = B64Url(JsonSerializer.SerializeToUtf8Bytes(claims));
        var data = $"{header}.{payload}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var sig = B64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
        return $"{data}.{sig}";
    }

    private static string B64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
