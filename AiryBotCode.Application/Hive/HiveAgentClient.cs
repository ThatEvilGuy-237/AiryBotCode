using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AiryBotCode.Application.Hive
{
    public record AgentReply(string? Text, bool Failed);

    public class HiveAgentClient
    {
        public const string AppKeyHeader = "X-Hive-App-Key";

        private readonly HttpClient _http;

        public HiveAgentClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<AgentReply> RunAsync(string runUrl, string appKey, ulong botId, ulong channelId, ulong authorId,
            string author, string content, IReadOnlyList<ForwardedImage>? images, CancellationToken cancellationToken = default)
        {
            var payload = JsonSerializer.Serialize(new
            {
                message = content,
                sessionId = channelId.ToString(),
                userId = authorId.ToString(),
                author,
                botId = botId.ToString(),
                images = images is { Count: > 0 }
                    ? images.Select(i => new { url = i.Url, name = i.Name, mime = i.Mime }).ToList()
                    : null,
            });

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, runUrl)
                {
                    Content = new StringContent(payload, Encoding.UTF8, "application/json"),
                };
                request.Headers.TryAddWithoutValidation(AppKeyHeader, appKey);

                using var response = await _http.SendAsync(request, cancellationToken);
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[HiveAgent] {runUrl} answered {(int)response.StatusCode}: {Shorten(body)}");
                    return new AgentReply(null, true);
                }
                return Read(body);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                Console.WriteLine($"[HiveAgent] {runUrl} failed: {ex.Message}");
                return new AgentReply(null, true);
            }
        }

        public static AgentReply Read(string body)
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object) return new AgentReply(null, true);
                var text = root.TryGetProperty("text", out var t) && t.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(t.GetString())
                    ? t.GetString()
                    : null;
                var failed = (root.TryGetProperty("hadErrors", out var h) && h.ValueKind == JsonValueKind.True)
                    || (root.TryGetProperty("error", out var e) && e.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(e.GetString()));
                return new AgentReply(text, failed && text is null);
            }
            catch (JsonException)
            {
                return new AgentReply(null, true);
            }
        }

        private static string Shorten(string body) => body.Length <= 300 ? body : body[..300] + "…";
    }
}
