using AiryBotCode.Application.DTOs;
using AiryBotCode.Domain.database;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Interfaces.Service
{
    public interface IConversationManagerService
    {
        Task<ConversationContext> GetOrCreateConversationContextAsync(ulong channelId, ulong authorId, string authorDisplayName, ulong botId, List<SocketUser> mentionedUsers);
        Task SaveMessagesAsync(ConversationContext context, Message userMessage, Message aiResponse);
        Task<string> GenerateAndSaveUserSummaryAsync(ulong userId, ulong guildId);
    }
}
