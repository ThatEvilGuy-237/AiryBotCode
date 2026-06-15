using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class ChannelWebhookRepository : IChannelWebhookRepository
    {
        private readonly AIDbContext _context;

        public ChannelWebhookRepository(AIDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChannelWebhook>> GetForBotAsync(ulong botId)
        {
            return await _context.ChannelWebhooks.AsNoTracking()
                .Where(c => c.BotId == botId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<ChannelWebhook?> GetForChannelAsync(ulong botId, ulong channelId)
        {
            return await _context.ChannelWebhooks.AsNoTracking()
                .FirstOrDefaultAsync(c => c.BotId == botId && c.ChannelId == channelId && c.Enabled);
        }

        public async Task<ChannelWebhook> AddAsync(ChannelWebhook link)
        {
            await _context.ChannelWebhooks.AddAsync(link);
            await _context.SaveChangesAsync();
            return link;
        }

        public async Task<bool> UpdateAsync(ChannelWebhook link)
        {
            var row = await _context.ChannelWebhooks
                .FirstOrDefaultAsync(c => c.Id == link.Id && c.BotId == link.BotId);
            if (row == null) return false;

            row.ChannelId = link.ChannelId;
            row.Name = link.Name;
            row.WebhookUrl = link.WebhookUrl;
            row.Secret = link.Secret;
            row.Mode = link.Mode;
            row.Enabled = link.Enabled;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(ulong botId, int id)
        {
            var row = await _context.ChannelWebhooks
                .FirstOrDefaultAsync(c => c.Id == id && c.BotId == botId);
            if (row == null) return false;
            _context.ChannelWebhooks.Remove(row);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
