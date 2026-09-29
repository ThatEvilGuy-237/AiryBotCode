using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;

namespace AiryBotCode.Application.Services
{
    public class WebhookChatService
    {
        public const string AgentMode = "agent";

        private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(30) };
        private readonly IChannelWebhookRepository _repository;
        private readonly HiveAgentClient _agents;

        private static readonly string[] Offline =
        {
            "I heard you, I just can't think right now.\n-# my servers are down, try me again later",
            "Your ping landed. My brain didn't.\n-# servers are down, this is the answering machine",
            "…nothing. Not you, it's me.\n-# my servers are down right now",
        };

        private static string OfflineReply() => Offline[Random.Shared.Next(Offline.Length)];

        public WebhookChatService(IChannelWebhookRepository repository, HiveAgentClient agents)
        {
            _repository = repository;
            _agents = agents;
        }

        public async Task<string?> TryForwardAsync(ulong botId, ulong channelId, ulong authorId, string author, string content,
            IReadOnlyList<ForwardedImage>? images = null,
            Func<IDisposable>? beginTyping = null)
        {
            var link = await _repository.GetForChannelAsync(botId, channelId);
            if (link == null) return null;

            using var typing = beginTyping?.Invoke();

            if (link.Mode == AgentMode)
            {
                if (string.IsNullOrWhiteSpace(link.Secret))
                {
                    Console.WriteLine($"[HiveAgent] channel {channelId} runs an agent but has no app key.");
                    return OfflineReply();
                }
                var reply = await _agents.RunAsync(link.WebhookUrl, link.Secret, botId, channelId, authorId, author, content, images);
                return reply.Text ?? (reply.Failed ? OfflineReply() : null);
            }

            var payload = JsonSerializer.Serialize(new
            {
                body = content,
                author,
                userId = authorId.ToString(),
                channelId = channelId.ToString(),
                botId = botId.ToString(),
                images = (images != null && images.Count > 0)
                    ? images.Select(i => new { url = i.Url, name = i.Name, mime = i.Mime }).ToList()
                    : null,
            });
            var bytes = Encoding.UTF8.GetBytes(payload);

            var baseUrl = link.WebhookUrl.Split('?')[0].TrimEnd('/');
            var sep = link.WebhookUrl.Contains('?') ? '&' : '?';
            var sendUrl = $"{link.WebhookUrl}{sep}mode=async";

            string? runId;
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Post, sendUrl) { Content = new ByteArrayContent(bytes) };
                req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                if (!string.IsNullOrEmpty(link.Secret))
                    req.Headers.TryAddWithoutValidation("X-Hive-Signature", Sign(link.Secret, bytes));

                using var res = await Http.SendAsync(req);
                var body = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Webhook] async send to {sendUrl} returned {(int)res.StatusCode}: {body}");
                    return OfflineReply();
                }
                runId = ReadString(body, "runId");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Webhook] forward to {link.WebhookUrl} failed: {ex.Message}");
                return OfflineReply();
            }

            if (string.IsNullOrEmpty(runId)) return OfflineReply();

            var resultUrl = $"{baseUrl}/result";
            var deadline = DateTime.UtcNow + TimeSpan.FromMinutes(10);
            while (DateTime.UtcNow < deadline)
            {
                await Task.Delay(2000);
                try
                {
                    var pollBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { runId }));
                    using var preq = new HttpRequestMessage(HttpMethod.Post, resultUrl) { Content = new ByteArrayContent(pollBytes) };
                    preq.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    if (!string.IsNullOrEmpty(link.Secret))
                        preq.Headers.TryAddWithoutValidation("X-Hive-Signature", Sign(link.Secret, pollBytes));

                    using var pres = await Http.SendAsync(preq);
                    if (!pres.IsSuccessStatusCode) continue;
                    var pbody = await pres.Content.ReadAsStringAsync();
                    var status = (ReadString(pbody, "status") ?? "").ToLowerInvariant();
                    if (status is "completed") return ExtractReply(pbody);
                    if (status is "completedwitherrors") return ExtractReply(pbody) ?? OfflineReply();
                    if (status is "failed" or "cancelled") return OfflineReply();
                }
                catch
                {
                }
            }

            Console.WriteLine($"[Webhook] run {runId} did not finish within the poll window.");
            return OfflineReply();
        }

        private static string? ReadString(string json, string prop)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                    doc.RootElement.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String)
                    return v.GetString();
            }
            catch
            {
            }
            return null;
        }

        private static string Sign(string secret, byte[] body)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return "sha256=" + Convert.ToHexString(hmac.ComputeHash(body)).ToLowerInvariant();
        }

        private static string? ExtractReply(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object) return null;

                if (root.TryGetProperty("finalOutput", out var fo) && fo.ValueKind == JsonValueKind.Object)
                {
                    if (TextFromOutput(fo) is { } fromOutput) return fromOutput;
                }

                if (TextFromOutput(root) is { } fromRoot) return fromRoot;
            }
            catch
            {
            }
            return null;
        }

        private static string? TextFromOutput(JsonElement obj)
        {
            foreach (var key in new[] { "text", "reply" })
                if (obj.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrWhiteSpace(v.GetString()))
                    return v.GetString();

            if (obj.TryGetProperty("message", out var msg))
            {
                if (msg.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(msg.GetString()))
                    return msg.GetString();
                if (msg.ValueKind == JsonValueKind.Object && msg.TryGetProperty("content", out var content))
                {
                    if (content.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(content.GetString()))
                        return content.GetString();
                    if (content.ValueKind == JsonValueKind.Array)
                    {
                        var sb = new StringBuilder();
                        foreach (var part in content.EnumerateArray())
                            if (part.ValueKind == JsonValueKind.Object &&
                                part.TryGetProperty("text", out var pt) && pt.ValueKind == JsonValueKind.String)
                                sb.Append(pt.GetString());
                        if (sb.Length > 0) return sb.ToString();
                    }
                }
            }
            return null;
        }
    }
}
