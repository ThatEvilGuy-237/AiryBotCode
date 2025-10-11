using AiryBotCode.Domain.database;

namespace AiryBotCode.Infrastructure.Database.Interfaces
{
    public interface IChannelConversationRepository : IEvilRepository<ChannelConversation>
    {
        Task<ChannelConversation?> GetByChannelIdAsync(ulong channelId);
    }
}
