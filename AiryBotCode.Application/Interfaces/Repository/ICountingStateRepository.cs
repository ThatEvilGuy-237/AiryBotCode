using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface ICountingStateRepository
    {
        Task<CountingState?> GetAsync(ulong botId, ulong channelId);
        Task SaveAsync(CountingState state);
    }
}
