using AiryBotCode.Domain.database;


namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface IMessageRepository : IEvilRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId);
        Task<IEnumerable<Message>> GetMessagesBychannelIdAsync(int channelId);
        Task<IEnumerable<Message>> GetMessagesByUserIdAsync(ulong userId);
        Task<List<Message>> GetAndPruneConversationHistoryAsync(int conversationId, int historyLimit = 6, int pruneCount = 2);
        Task<List<string>> GetUserMessageHistoryForSummaryAsync(ulong userId, int messageLimit = 100);
    }
}
