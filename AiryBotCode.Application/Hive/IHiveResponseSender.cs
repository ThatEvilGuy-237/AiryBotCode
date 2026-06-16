namespace AiryBotCode.Application.Hive
{
    /// <summary>
    /// Sends a user's answer back up the Hive tools WS as an <c>effect_response</c>,
    /// correlated by effect id — the return half of an await-mode tool (e.g. the user
    /// tapped a button raised by <c>ask_user</c>). Implemented by the live WS listener,
    /// which owns the active socket. Returns false when no Hive connection is open.
    /// </summary>
    public interface IHiveResponseSender
    {
        bool IsConnected { get; }

        Task<bool> SendAnswerAsync(string effectId, string answer, string? sessionId, string? userId, CancellationToken ct = default);
    }
}
