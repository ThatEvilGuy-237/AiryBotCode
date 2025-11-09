using AiryBotCode.Domain.database;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Interfaces.Service
{
    public interface IChatUserService
    {
        Task<ChatUser> GetOrCreateChatUserAsync(ulong userId, string userName, ChatRole role, string defaultAiOpinion = "A friendly Discord user");
        Task<ChatUser> GetOrCreateSystemUserAsync();
        Task<ChatUser> GetOrCreateAiUserAsync(ulong botId);
        Task UpdateUserAiOpinionAsync(ChatUser user, string aiOpinion);
    }
}
