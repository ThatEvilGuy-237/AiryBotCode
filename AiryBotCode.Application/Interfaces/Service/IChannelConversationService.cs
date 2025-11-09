using AiryBotCode.Domain.database;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Interfaces.Service
{
    public interface IChannelConversationService
    {
        Task<ChannelConversation> GetOrCreateChannelConversationAsync(ulong channelId);
    }
}
