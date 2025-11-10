using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Domain.database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Services.Database
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;

        public MessageService(IMessageRepository messageRepo)
        {
            _messageRepo = messageRepo;
        }

        public async Task<List<Message>> GetAndManageConversationHistoryAsync(int conversationId)
        {
            return await _messageRepo.GetAndPruneConversationHistoryAsync(conversationId);
        }

        public async Task SaveMessageAsync(Message message)
        {
            await _messageRepo.AddAsync(message);
            await _messageRepo.SaveChangesAsync();
        }

        public async Task<List<Message>> GetMessagesByUserIdAsync(ulong userId)
        {
            return (List<Message>)await _messageRepo.GetMessagesByUserIdAsync(userId);
        }

        public async Task<List<string>> GetUserMessageHistoryForSummaryAsync(ulong userId, int messageLimit = 100)
        {
            return await _messageRepo.GetUserMessageHistoryForSummaryAsync(userId, messageLimit);
        }
    }
}
