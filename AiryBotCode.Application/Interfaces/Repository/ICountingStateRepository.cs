using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface ICountingStateRepository
    {
        Task<CountingState?> GetAsync(ulong botId, ulong channelId);
        Task SaveAsync(CountingState state);
        // All counting channels for a bot, newest-updated first (read-only, for the panel).
        Task<IReadOnlyList<CountingState>> ListByBotAsync(ulong botId);
    }
}
