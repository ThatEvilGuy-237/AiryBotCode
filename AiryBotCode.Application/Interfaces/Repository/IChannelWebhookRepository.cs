using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    /// <summary>Channel → webhook links per bot (the "chat" routing).</summary>
    public interface IChannelWebhookRepository
    {
        Task<List<ChannelWebhook>> GetForBotAsync(ulong botId);
        Task<ChannelWebhook?> GetForChannelAsync(ulong botId, ulong channelId);
        Task<ChannelWebhook> AddAsync(ChannelWebhook link);
        Task<bool> UpdateAsync(ChannelWebhook link);
        Task<bool> DeleteAsync(ulong botId, int id);
    }
}
