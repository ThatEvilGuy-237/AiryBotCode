using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class ChannelDecisionWatchRepository : IChannelDecisionWatchRepository
    {
        private readonly AIDbContext _context;

        public ChannelDecisionWatchRepository(AIDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChannelDecisionWatch>> GetForBotAsync(ulong botId) =>
            await _context.ChannelDecisionWatches.AsNoTracking()
                .Where(w => w.BotId == botId)
                .OrderBy(w => w.Name)
                .ToListAsync();

        public async Task<List<ChannelDecisionWatch>> GetForChannelAsync(ulong botId, ulong channelId) =>
            await _context.ChannelDecisionWatches.AsNoTracking()
                .Where(w => w.BotId == botId && w.ChannelId == channelId && w.Enabled)
                .OrderBy(w => w.Id)
                .ToListAsync();

        public async Task<ChannelDecisionWatch?> GetAsync(ulong botId, int id) =>
            await _context.ChannelDecisionWatches.AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id && w.BotId == botId);

        public async Task<ChannelDecisionWatch> AddAsync(ChannelDecisionWatch watch)
        {
            await _context.ChannelDecisionWatches.AddAsync(watch);
            await _context.SaveChangesAsync();
            return watch;
        }

        public async Task<bool> UpdateAsync(ChannelDecisionWatch watch)
        {
            var row = await _context.ChannelDecisionWatches.FirstOrDefaultAsync(w => w.Id == watch.Id && w.BotId == watch.BotId);
            if (row == null) return false;
            row.ChannelId = watch.ChannelId;
            row.Name = watch.Name;
            row.ListenerUrl = watch.ListenerUrl;
            row.Secret = watch.Secret;
            row.QuestionKey = watch.QuestionKey;
            row.ActionsJson = watch.ActionsJson;
            row.UnsureAction = watch.UnsureAction;
            row.AlertChannelId = watch.AlertChannelId;
            row.Enabled = watch.Enabled;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(ulong botId, int id)
        {
            var row = await _context.ChannelDecisionWatches.FirstOrDefaultAsync(w => w.Id == id && w.BotId == botId);
            if (row == null) return false;
            _context.ChannelDecisionWatches.Remove(row);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
