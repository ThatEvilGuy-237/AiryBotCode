using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class AiryChannelRepository : IAiryChannelRepository
    {
        private readonly AIDbContext _context;

        public AiryChannelRepository(AIDbContext context)
        {
            _context = context;
        }

        public async Task<List<AiryChannel>> GetForBotAsync(ulong botId) =>
            await _context.AiryChannels.AsNoTracking()
                .Where(c => c.BotId == botId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

        public async Task<AiryChannel?> GetForChannelAsync(ulong botId, ulong channelId) =>
            await _context.AiryChannels.AsNoTracking()
                .FirstOrDefaultAsync(c => c.BotId == botId && c.ChannelId == channelId && c.Enabled);

        public async Task<AiryChannel?> GetAsync(ulong botId, int id) =>
            await _context.AiryChannels.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.BotId == botId);

        public async Task<AiryChannel> AddAsync(AiryChannel channel)
        {
            await _context.AiryChannels.AddAsync(channel);
            await _context.SaveChangesAsync();
            return channel;
        }

        public async Task<bool> UpdateAsync(AiryChannel channel)
        {
            var row = await _context.AiryChannels.FirstOrDefaultAsync(c => c.Id == channel.Id && c.BotId == channel.BotId);
            if (row is null) return false;
            row.AnswerMode = channel.AnswerMode;
            row.Enabled = channel.Enabled;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(ulong botId, int id)
        {
            var row = await _context.AiryChannels.FirstOrDefaultAsync(c => c.Id == id && c.BotId == botId);
            if (row is null) return false;
            _context.AiryChannels.Remove(row);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RecordDecisionAsync(int id, DateTime at, string? decision, string? error)
        {
            await _context.AiryChannels.Where(c => c.Id == id).ExecuteUpdateAsync(s => s
                .SetProperty(c => c.LastDecisionAt, at)
                .SetProperty(c => c.LastDecision, decision)
                .SetProperty(c => c.LastDecisionError, error));
        }
    }
}
