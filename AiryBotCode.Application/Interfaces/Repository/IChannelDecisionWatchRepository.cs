using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface IChannelDecisionWatchRepository
    {
        Task<List<ChannelDecisionWatch>> GetForBotAsync(ulong botId);
        Task<List<ChannelDecisionWatch>> GetForChannelAsync(ulong botId, ulong channelId);
        Task<ChannelDecisionWatch?> GetAsync(ulong botId, int id);
        Task<ChannelDecisionWatch> AddAsync(ChannelDecisionWatch watch);
        Task<bool> UpdateAsync(ChannelDecisionWatch watch);
        Task<bool> DeleteAsync(ulong botId, int id);
    }
}
