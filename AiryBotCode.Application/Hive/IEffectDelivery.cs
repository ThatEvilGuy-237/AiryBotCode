namespace AiryBotCode.Application.Hive
{
    /// <summary>
    /// Delivers a Hive effect's message to Discord. Kept as an interface so the
    /// listener's parse/route/deliver path is testable without a live Discord
    /// client (the Infrastructure impl posts via the bot's socket client).
    /// </summary>
    public interface IEffectDelivery
    {
        Task SendAsync(ulong channelId, string text, CancellationToken ct = default);
    }
}
