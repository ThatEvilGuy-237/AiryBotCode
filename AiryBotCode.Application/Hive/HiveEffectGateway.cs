namespace AiryBotCode.Application.Hive
{
    /// <summary>
    /// Singleton indirection between the long-lived WS listener (created at bot start)
    /// and request-scoped consumers (the button handler). The listener binds itself
    /// here once connected; <see cref="IHiveResponseSender"/> consumers resolve this
    /// and delegate to whatever listener is currently bound. Reports not-connected and
    /// returns false when nothing is bound (Hive effects opt-in is off). Routing back
    /// is by effect id, so any open subscriber connection can carry any answer.
    /// </summary>
    public sealed class HiveEffectGateway : IHiveResponseSender
    {
        private volatile IHiveResponseSender? _inner;

        public void Bind(IHiveResponseSender inner) => _inner = inner;

        public bool IsConnected => _inner?.IsConnected ?? false;

        public Task<bool> SendAnswerAsync(string effectId, string answer, string? sessionId, string? userId, CancellationToken ct = default)
            => _inner?.SendAnswerAsync(effectId, answer, sessionId, userId, ct) ?? Task.FromResult(false);

        public Task<bool> SendEventAsync(string type, object payload, string? sessionId, CancellationToken ct = default)
            => _inner?.SendEventAsync(type, payload, sessionId, ct) ?? Task.FromResult(false);
    }
}
