using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository

{
    public interface IChannelConversationRepository : IEvilRepository<ChannelConversation>
    {
        Task<ChannelConversation?> GetByChannelIdAsync(ulong channelId);
    }
}
