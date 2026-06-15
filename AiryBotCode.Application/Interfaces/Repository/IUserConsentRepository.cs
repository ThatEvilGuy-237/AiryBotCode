namespace AiryBotCode.Application.Interfaces.Repository
{
    /// <summary>Per-bot record of which users accepted Airy's data-use notice.</summary>
    public interface IUserConsentRepository
    {
        Task<bool> HasConsentAsync(ulong botId, ulong userId);
        /// <summary>Record consent (idempotent — a second grant is a no-op).</summary>
        Task GrantAsync(ulong botId, ulong userId);
    }
}
