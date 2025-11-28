using AiryBotCode.Domain.database;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AiryBotCode.Infrastructure.Database.Repository.ChatHistory
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
        public async Task<IEnumerable<Message>> GetMessagesByUserIdAsync(ulong userId)
        {
            return await _dbSet.Where(m => m.UserId == userId).ToListAsync();
        }

        public async Task<List<Message>> GetAndPruneConversationHistoryAsync(int conversationId, int historyLimit = 6, int pruneCount = 2)
        {
            var history = await _dbSet.Where(m => m.ChannelConversationId == conversationId).OrderBy(m => m.CreatedAt).ToListAsync();

            if (history.Count >= historyLimit)
            {
                var messagesToDelete = history.Take(pruneCount).ToList();
                foreach (var msg in messagesToDelete)
                {
                    _dbSet.Remove(msg);
                }
                await _context.SaveChangesAsync();
                history.RemoveRange(0, pruneCount);
            }

            return history;
        }

        public async Task<List<string>> GetUserMessageHistoryForSummaryAsync(ulong userId, int messageLimit = 100)
        {
            return await _context.Messages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(messageLimit)
                .Select(m => m.Context)
                .ToListAsync();
        }
    }
}
