using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface IAiryChannelRepository
    {
        Task<List<AiryChannel>> GetForBotAsync(ulong botId);
        Task<AiryChannel?> GetForChannelAsync(ulong botId, ulong channelId);
        Task<AiryChannel?> GetAsync(ulong botId, int id);
        Task<AiryChannel> AddAsync(AiryChannel channel);
        Task<bool> UpdateAsync(AiryChannel channel);
        Task<bool> DeleteAsync(ulong botId, int id);
        Task RecordDecisionAsync(int id, DateTime at, string? decision, string? error);
    }
}
