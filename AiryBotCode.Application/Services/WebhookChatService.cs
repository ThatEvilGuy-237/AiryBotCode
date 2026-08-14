using System.Linq;
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

        /// <summary>What to say when the channel IS linked but the Hive gave us nothing —
        /// the run errored, timed out, or the model is unreachable (no credits, provider
        /// down). Silence reads as "the bot is broken/ignoring me"; this says which it is,
        /// quietly, in Discord subtext. An UNLINKED channel still gets nothing at all.</summary>
        private static readonly string[] Offline =
        {
            "I heard you — I just can't think right now.\n-# my servers are down, try me again later",
            "Your ping landed. My brain didn't.\n-# servers are down — this is the answering machine",
            "…nothing. Not you, it's me.\n-# my servers are down right now",
        };

        private static string OfflineReply() => Offline[Random.Shared.Next(Offline.Length)];

        public WebhookChatService(IChannelWebhookRepository repository)
        {
            _repository = repository;
        }

        /// <summary>Forward a channel message to its linked webhook. Returns reply text, or null.
        /// <paramref name="beginTyping"/> (if given) is invoked once the channel is confirmed
        /// linked and its result disposed after the reply — used to show the "typing…" indicator
        /// while the Hive flow runs, without flickering on unlinked channels.</summary>
        public async Task<string?> TryForwardAsync(ulong botId, ulong channelId, ulong authorId, string author, string content,
            IReadOnlyList<Hive.ForwardedImage>? images = null,
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
                // WHICH BOT this run belongs to. The Hive threads it into the agent
                // run's effect context, and Wraith's tools-WS then delivers the reply
                // ONLY to the listener that subscribed as this bot — which is what
                // stops every bot on the hub posting the same answer.
                botId = botId.ToString(),
                // Image attachments for the agent's vision intake. MUST use lowercase
                // keys (url/name/mime): the Hive's FlowRunner.ReadImages reads them
                // case-sensitively, but the ForwardedImage record serializes PascalCase
                // (Url/Name/Mime) and would be silently dropped. Additive: Hive ignores
                // it when empty.
                images = (images != null && images.Count > 0)
                    ? images.Select(i => new { url = i.Url, name = i.Name, mime = i.Mime }).ToList()
                    : null,
            });
            var bytes = Encoding.UTF8.GetBytes(payload);

            // Fire async — never hold one long synchronous request open. A slow flow
            // would miss the sync window and its reply would never reach Discord even
            // though the run finished. We get a runId immediately, then poll the result
            // endpoint (short requests) until the run is done.
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

            // Poll the signed result endpoint until the run reaches a terminal state.
            // Window is generous so an interactive run that BLOCKS on an ask_user button
            // (the agent suspends until the user taps) is still being polled when the
            // answer comes back — the old 3 min raced the button press.
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
                    // A CLEAN completion with no text is left silent on purpose: the agent
                    // may have answered through a say-effect instead, and an "I'm offline"
                    // posted on top of a real reply is worse than saying nothing. Only
                    // completedwitherrors — the shape of a dead model, e.g. the provider
                    // answering 429 — gets the stand-in.
                    if (status is "completed") return ExtractReply(pbody);
                    if (status is "completedwitherrors") return ExtractReply(pbody) ?? OfflineReply();
                    if (status is "failed" or "cancelled") return OfflineReply();
                    // pending / running → keep polling
                }
                catch { /* transient — keep polling until the deadline */ }
            }

            Console.WriteLine($"[Webhook] run {runId} did not finish within the poll window.");
            return OfflineReply();
        }

        // Pull a top-level string property from a JSON object response.
        private static string? ReadString(string json, string prop)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                    doc.RootElement.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String)
                    return v.GetString();
            }
            catch { /* not JSON */ }
            return null;
        }

        private static string Sign(string secret, byte[] body)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return "sha256=" + Convert.ToHexString(hmac.ComputeHash(body)).ToLowerInvariant();
        }

        // Pull a human-facing reply from the response. Canonical source is the
        // FlowRun's finalOutput.text, but we stay tolerant of layout drift: the
        // Hive's agent output also carries message.content (string or content-part
        // array), and plain { text } / { message } / { reply } shapes are accepted
        // too. Resilient on purpose — a shape change upstream shouldn't silently
        // swallow the bot's reply.
        private static string? ExtractReply(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object) return null;

                // FlowRun shape: dig into finalOutput first (text, then message.content).
                if (root.TryGetProperty("finalOutput", out var fo) && fo.ValueKind == JsonValueKind.Object)
                {
                    if (TextFromOutput(fo) is { } fromOutput) return fromOutput;
                }

                // Flat shapes: { text } / { reply }, or { message: { content } } / { message: "..." }.
                if (TextFromOutput(root) is { } fromRoot) return fromRoot;
            }
            catch { /* not JSON or no reply field */ }
            return null;
        }

        // Best-effort text from an output object: text → reply → message.content → message (string).
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
                    // content-part array: [{ type:"text", text:"…" }, …]
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
