using AiryBotCode.Domain.database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Interfaces.Service
{
    public interface IMessageService
    {
        Task<List<Message>> GetAndManageConversationHistoryAsync(int conversationId);
        Task SaveMessageAsync(Message message);
        Task SaveMessagesAsync(Message userMessage, Message aiResponse);
        Task<List<Message>> GetMessagesByUserIdAsync(ulong userId);
        Task<List<string>> GetUserMessageHistoryForSummaryAsync(ulong userId, int messageLimit = 100);
    }
}
