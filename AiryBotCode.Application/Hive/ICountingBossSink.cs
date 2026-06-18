namespace AiryBotCode.Application.Hive
{
    /// <summary>
    /// Receives a mini-boss's expected answer from the Hive (Airy generates the
    /// puzzle, then sends its answer back over the tools WS as a
    /// <c>counting_boss_answer</c> frame). The bot stores it so it can judge
    /// players' attempts locally without an AI round-trip per guess.
    /// </summary>
    public interface ICountingBossSink
    {
        Task SetBossAnswerAsync(ulong channelId, double answer, CancellationToken ct = default);
    }
}
