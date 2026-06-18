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
    public sealed class HiveEffectListener : IHiveResponseSender
    {
        private readonly string _wsUrl;
        private readonly MessagePacer _pacer;
        private readonly IAskDelivery? _askDelivery;
        private readonly ICountingBossSink? _bossSink;
        private readonly Action<string>? _log;

        // The currently-connected socket, set on each (re)connect and cleared on drop.
        // Used to send await-mode answers (effect_response) back up the same WS.
        private ClientWebSocket? _activeSocket;
        private readonly SemaphoreSlim _sendLock = new(1, 1);

        public HiveEffectListener(string wsUrl, IEffectDelivery delivery, Action<string>? log = null, IAskDelivery? askDelivery = null, ICountingBossSink? bossSink = null)
            : this(wsUrl, new MessagePacer(delivery), log, askDelivery, bossSink) { }

        // Test/advanced ctor: supply a pacer (e.g. with a fake clock).
        public HiveEffectListener(string wsUrl, MessagePacer pacer, Action<string>? log = null, IAskDelivery? askDelivery = null, ICountingBossSink? bossSink = null)
        {
            _wsUrl = wsUrl;
            _pacer = pacer;
            _askDelivery = askDelivery;
            _bossSink = bossSink;
            _log = log;
        }

        public bool IsConnected => _activeSocket?.State == WebSocketState.Open;

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

                    _activeSocket = ws;
                    try { await ReceiveLoopAsync(ws, ct); }
                    finally { _activeSocket = null; }
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

                // Fire-and-forget: pacing must not block receiving the next frame.
                _ = HandleMessageAsync(sb.ToString(), ct);
            }
        }

        /// <summary>Parse one WS frame and (if deliverable) hand it to the pacer.
        /// Returns the delivery task (tests await it); the receive loop discards it.
        /// Never throws — a malformed frame is logged and skipped.</summary>
        public Task HandleMessageAsync(string json, CancellationToken ct = default)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var frameType = root.TryGetProperty("type", out var t) ? t.GetString() : null;

                // Inbound mini-boss answer: Airy generated a puzzle and sends back the
                // expected answer for the bot to judge against. Frame:
                //   { type:"counting_boss_answer", context:{ sessionId:<channelId> }, payload:{ answer:<number> } }
                if (frameType == "counting_boss_answer") return HandleBossAnswer(root, ct);

                if (frameType != "effect") return Task.CompletedTask;

                var call = root.GetProperty("call");
                var name = call.TryGetProperty("name", out var n) ? n.GetString() : null;
                var effectId = call.TryGetProperty("id", out var idEl) ? idEl.GetString() : null;
                var args = call.TryGetProperty("arguments", out var a) ? a : default;
                var message = args.ValueKind == JsonValueKind.Object && args.TryGetProperty("message", out var m)
                    ? m.GetString() : null;
                double? delay = args.ValueKind == JsonValueKind.Object
                    && args.TryGetProperty("delaySeconds", out var d) && d.ValueKind == JsonValueKind.Number
                    ? d.GetDouble() : null;
                var sessionId = root.TryGetProperty("context", out var c) && c.ValueKind == JsonValueKind.Object
                    && c.TryGetProperty("sessionId", out var s) ? s.GetString() : null;

                // ── await-mode ask_user: post the question with option buttons ──────
                // The user's tap is routed back as an effect_response by the button
                // handler (see ButtonPressHandler → IHiveResponseSender).
                if (name == "ask_user")
                {
                    var question = args.ValueKind == JsonValueKind.Object && args.TryGetProperty("question", out var q)
                        ? q.GetString() : null;
                    var options = new List<string>();
                    if (args.ValueKind == JsonValueKind.Object && args.TryGetProperty("options", out var opts)
                        && opts.ValueKind == JsonValueKind.Array)
                        foreach (var o in opts.EnumerateArray())
                            if (o.ValueKind == JsonValueKind.String) options.Add(o.GetString()!);

                    var userId = root.TryGetProperty("context", out var cc) && cc.ValueKind == JsonValueKind.Object
                        && cc.TryGetProperty("userId", out var u) ? u.GetString() : null;
                    var ask = AskRouter.Route(name, effectId, question, options, sessionId);
                    if (ask is null) return Task.CompletedTask;
                    if (_askDelivery is null) { _log?.Invoke("[HiveEffects] ask_user effect but no ask delivery configured"); return Task.CompletedTask; }
                    return _askDelivery.SendAskAsync(ask.ChannelId, ask.EffectId, ask.Question, ask.Options, userId, ct);
                }

                var intent = EffectRouter.Route(name, message, delay, sessionId);
                if (intent is null) return Task.CompletedTask;

                // The pacer spaces follow-ups from the previous actual send.
                return _pacer.Enqueue(intent.ChannelId, intent.Text, intent.DelaySeconds, ct);
            }
            catch (Exception ex)
            {
                _log?.Invoke($"[HiveEffects] bad effect frame skipped: {ex.Message}");
                return Task.CompletedTask;
            }
        }

        /// <summary>Send a user's answer back as an <c>effect_response</c>, correlated
        /// by effect id, over the live socket. Returns false if no connection is open.
        /// Serialized through a lock so concurrent button taps can't interleave frames.</summary>
        public async Task<bool> SendAnswerAsync(string effectId, string answer, string? sessionId, string? userId, CancellationToken ct = default)
        {
            var ws = _activeSocket;
            if (ws is null || ws.State != WebSocketState.Open) return false;

            var frame = JsonSerializer.Serialize(new
            {
                type = "effect_response",
                id = effectId,
                response = new { answer },
                context = new { sessionId, userId },
            });

            await _sendLock.WaitAsync(ct);
            try
            {
                if (ws.State != WebSocketState.Open) return false;
                await SendAsync(ws, frame, ct);
                _log?.Invoke($"[HiveEffects] effect_response sent for {effectId}");
                return true;
            }
            catch (Exception ex)
            {
                _log?.Invoke($"[HiveEffects] effect_response send failed: {ex.Message}");
                return false;
            }
            finally { _sendLock.Release(); }
        }

        // Parse a counting_boss_answer frame and hand the answer to the sink.
        private Task HandleBossAnswer(JsonElement root, CancellationToken ct)
        {
            if (_bossSink is null) return Task.CompletedTask;
            var sessionId = root.TryGetProperty("context", out var c) && c.ValueKind == JsonValueKind.Object
                && c.TryGetProperty("sessionId", out var s) ? s.GetString() : null;
            double? answer = root.TryGetProperty("payload", out var p) && p.ValueKind == JsonValueKind.Object
                && p.TryGetProperty("answer", out var a) && a.ValueKind == JsonValueKind.Number ? a.GetDouble() : null;
            if (sessionId is null || answer is null || !ulong.TryParse(sessionId, out var channelId))
                return Task.CompletedTask;
            return _bossSink.SetBossAnswerAsync(channelId, answer.Value, ct);
        }

        /// <summary>Send an arbitrary event frame upstream over the live socket.</summary>
        public async Task<bool> SendEventAsync(string type, object payload, string? sessionId, CancellationToken ct = default)
        {
            var ws = _activeSocket;
            if (ws is null || ws.State != WebSocketState.Open) return false;

            var frame = JsonSerializer.Serialize(new { type, payload, context = new { sessionId } });

            await _sendLock.WaitAsync(ct);
            try
            {
                if (ws.State != WebSocketState.Open) return false;
                await SendAsync(ws, frame, ct);
                _log?.Invoke($"[HiveEffects] event '{type}' sent");
                return true;
            }
            catch (Exception ex)
            {
                _log?.Invoke($"[HiveEffects] event '{type}' send failed: {ex.Message}");
                return false;
            }
            finally { _sendLock.Release(); }
        }

        private static Task SendAsync(ClientWebSocket ws, string json, CancellationToken ct) =>
            ws.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, ct);
    }
}
