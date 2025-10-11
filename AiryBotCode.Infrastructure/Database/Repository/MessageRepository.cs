using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Interfaces;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AiryBotCode.Infrastructure.Database.Repository
{
    internal class MessageRepository : EvilRepository<Message>, IMessageRepository
    {
        public MessageRepository(AIDbContext context) : base(context) { }
        public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId)
        {
            return await _dbSet.Where(m => m.ChannelConversationId == conversationId).ToListAsync();
        }
        public async Task<IEnumerable<Message>> GetMessagesBychannelIdAsync(int channelId)
        {
            return await _dbSet.Where(m => m.ChannelConversationId == channelId).ToListAsync();
        }
    }
}
