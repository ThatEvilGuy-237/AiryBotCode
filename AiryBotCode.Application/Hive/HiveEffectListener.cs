using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace AiryBotCode.Application.Hive
{
    /// <summary>
    /// Subscribes to the Hive tools websocket and delivers the agent's outbound
    /// "effect" messages (e.g. the <c>say</c> tool) back to Discord — the consumer
    /// half of the Hive effect passthrough. Host-agnostic: call <see cref="RunAsync"/>
    /// from wherever the bot is started; it connects, sends <c>subscribe_effects</c>,
    /// relays each <c>effect</c> frame, and reconnects with backoff on drop.
    /// </summary>
    public sealed class HiveEffectListener
    {
        private readonly string _wsUrl;
        private readonly IEffectDelivery _delivery;
        private readonly Action<string>? _log;

        public HiveEffectListener(string wsUrl, IEffectDelivery delivery, Action<string>? log = null)
        {
            _wsUrl = wsUrl;
            _delivery = delivery;
            _log = log;
        }

        public async Task RunAsync(CancellationToken ct)
        {
            var backoff = 2;
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var ws = new ClientWebSocket();
                    await ws.ConnectAsync(new Uri(_wsUrl), ct);
                    await SendAsync(ws, "{\"type\":\"subscribe_effects\"}", ct);
                    _log?.Invoke($"[HiveEffects] subscribed at {_wsUrl}");
                    backoff = 2;

                    await ReceiveLoopAsync(ws, ct);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _log?.Invoke($"[HiveEffects] connection error: {ex.Message}; retrying in {backoff}s");
                    try { await Task.Delay(TimeSpan.FromSeconds(backoff), ct); } catch { break; }
                    backoff = Math.Min(backoff * 2, 60);
                }
            }
        }

        private async Task ReceiveLoopAsync(ClientWebSocket ws, CancellationToken ct)
        {
            var buffer = new byte[64 * 1024];
            var sb = new StringBuilder();
            while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                sb.Clear();
                WebSocketReceiveResult result;
                do
                {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                    if (result.MessageType == WebSocketMessageType.Close)
                        return;
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                while (!result.EndOfMessage);

                await HandleMessageAsync(sb.ToString(), ct);
            }
        }

        /// <summary>Parse one WS frame, route a deliverable effect, and deliver it.
        /// Never throws — a malformed frame is logged and skipped. Public for tests.</summary>
        public async Task HandleMessageAsync(string json, CancellationToken ct = default)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (!root.TryGetProperty("type", out var t) || t.GetString() != "effect") return;

                var call = root.GetProperty("call");
                var name = call.TryGetProperty("name", out var n) ? n.GetString() : null;
                var args = call.TryGetProperty("arguments", out var a) ? a : default;
                var message = args.ValueKind == JsonValueKind.Object && args.TryGetProperty("message", out var m)
                    ? m.GetString() : null;
                double? delay = args.ValueKind == JsonValueKind.Object
                    && args.TryGetProperty("delaySeconds", out var d) && d.ValueKind == JsonValueKind.Number
                    ? d.GetDouble() : null;
                var sessionId = root.TryGetProperty("context", out var c) && c.ValueKind == JsonValueKind.Object
                    && c.TryGetProperty("sessionId", out var s) ? s.GetString() : null;

                var intent = EffectRouter.Route(name, message, delay, sessionId);
                if (intent is null) return;

                if (intent.DelaySeconds > 0)
                    await Task.Delay(TimeSpan.FromSeconds(intent.DelaySeconds), ct);
                await _delivery.SendAsync(intent.ChannelId, intent.Text, ct);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _log?.Invoke($"[HiveEffects] bad effect frame skipped: {ex.Message}");
            }
        }

        private static Task SendAsync(ClientWebSocket ws, string json, CancellationToken ct) =>
            ws.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, ct);
    }
}
