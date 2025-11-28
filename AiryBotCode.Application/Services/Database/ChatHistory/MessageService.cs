using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Services.Database.ChatHistory
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
            return await _messageRepo.GetAndPruneConversationHistoryAsync(conversationId, 15);
        }

        public async Task SaveMessageAsync(Message message)
        {
            await _messageRepo.AddAsync(message);
            await _messageRepo.SaveChangesAsync();
        }

        public async Task SaveMessagesAsync(Message userMessage, Message aiResponse)
        {
            await _messageRepo.AddAsync(userMessage);
            await _messageRepo.AddAsync(aiResponse);
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
