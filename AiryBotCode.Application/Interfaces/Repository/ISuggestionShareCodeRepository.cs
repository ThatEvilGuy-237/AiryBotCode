using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    /// <summary>Single-row store for the suggestions-board share code.</summary>
    public interface ISuggestionShareCodeRepository
    {
        Task<SuggestionShareCode?> GetAsync();
        // Replace whatever code is stored (single-row semantics).
        Task SetAsync(string code);
    }
}
