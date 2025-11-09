using AiryBotCode.Domain.database;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class ChannelConversationRepository : EvilRepository<ChannelConversation>, IChannelConversationRepository
    {
        public ChannelConversationRepository(AIDbContext context) : base(context) { }

        public async Task<ChannelConversation?> GetByChannelIdAsync(ulong channelId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.ChannelId == channelId);
        }
    }
}
