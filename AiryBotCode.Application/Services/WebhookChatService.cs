using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AiryBotCode.Application.Interfaces.Repository;

namespace AiryBotCode.Application.Services
{
    /// <summary>
    /// The "chat" bridge: when a message lands in a channel linked to a webhook,
    /// sign + POST it (mirroring the Hive's webhook contract — HMAC-SHA256 in
    /// X-Hive-Signature, mode via ?mode=) and relay any reply text back.
    /// </summary>
    public class WebhookChatService
    {
        private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(30) };
        private readonly IChannelWebhookRepository _repository;

        public WebhookChatService(IChannelWebhookRepository repository)
        {
            _repository = repository;
        }

        /// <summary>Forward a channel message to its linked webhook. Returns reply text, or null.
        /// <paramref name="beginTyping"/> (if given) is invoked once the channel is confirmed
        /// linked and its result disposed after the reply — used to show the "typing…" indicator
        /// while the Hive flow runs, without flickering on unlinked channels.</summary>
        public async Task<string?> TryForwardAsync(ulong botId, ulong channelId, ulong authorId, string author, string content,
            Func<IDisposable>? beginTyping = null)
        {
            var link = await _repository.GetForChannelAsync(botId, channelId);
            if (link == null) return null;

            // Linked → show the bot as typing until we've sent the reply back.
            using var typing = beginTyping?.Invoke();

            var payload = JsonSerializer.Serialize(new
            {
                body = content,
                author,
                userId = authorId.ToString(),   // Discord user id → per-user memory in the Hive
                channelId = channelId.ToString(),
            });
            var bytes = Encoding.UTF8.GetBytes(payload);

            var sep = link.WebhookUrl.Contains('?') ? '&' : '?';
            var url = $"{link.WebhookUrl}{sep}mode={(link.Mode == "async" ? "async" : "sync")}";

            using var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ByteArrayContent(bytes),
            };
            req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            if (!string.IsNullOrEmpty(link.Secret))
                req.Headers.TryAddWithoutValidation("X-Hive-Signature", Sign(link.Secret, bytes));

            try
            {
                using var res = await Http.SendAsync(req);
                var body = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode) return null;
                return ExtractReply(body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Webhook] forward to {link.WebhookUrl} failed: {ex.Message}");
                return null;
            }
        }

        private static string Sign(string secret, byte[] body)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return "sha256=" + Convert.ToHexString(hmac.ComputeHash(body)).ToLowerInvariant();
        }

        // Pull a human-facing reply from the response. The Hive returns a FlowRun
        // (finalOutput.text once it has run); plain { text } / { message } also work.
        private static string? ExtractReply(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object) return null;

                if (root.TryGetProperty("finalOutput", out var fo) && fo.ValueKind == JsonValueKind.Object &&
                    fo.TryGetProperty("text", out var ft) && ft.ValueKind == JsonValueKind.String)
                    return ft.GetString();

                foreach (var key in new[] { "text", "message", "reply" })
                    if (root.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String)
                        return v.GetString();
            }
            catch { /* not JSON or no reply field */ }
            return null;
        }
    }
}
