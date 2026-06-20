using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface ISuggestionRepository
    {
        // Newest first.
        Task<IReadOnlyList<Suggestion>> GetAllAsync();
        Task<Suggestion?> GetAsync(int id);
        Task<Suggestion> AddAsync(Suggestion suggestion);
        Task UpdateAsync(Suggestion suggestion);
        Task<bool> DeleteAsync(int id);

        // Suggestions with no maintainer review yet (ResponseWhy is null/empty) —
        // powers the standalone "what still needs answering" checker.
        Task<IReadOnlyList<Suggestion>> GetUnansweredAsync();
    }
}
