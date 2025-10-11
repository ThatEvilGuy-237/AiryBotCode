using AiryBotCode.Domain.database;


namespace AiryBotCode.Infrastructure.Database.Interfaces
{
    public interface IMessageRepository : IEvilRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId);
        Task<IEnumerable<Message>> GetMessagesBychannelIdAsync(int channelId);
    }
}
