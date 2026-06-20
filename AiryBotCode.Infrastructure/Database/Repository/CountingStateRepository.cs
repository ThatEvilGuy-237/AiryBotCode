using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class CountingStateRepository : ICountingStateRepository
    {
        private readonly AIDbContext _context;

        public CountingStateRepository(AIDbContext context)
        {
            _context = context;
        }

        public Task<CountingState?> GetAsync(ulong botId, ulong channelId) =>
            _context.CountingStates.AsNoTracking()
                .FirstOrDefaultAsync(x => x.BotId == botId && x.ChannelId == channelId);

        public async Task<IReadOnlyList<CountingState>> ListByBotAsync(ulong botId) =>
            await _context.CountingStates.AsNoTracking()
                .Where(x => x.BotId == botId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();

        public async Task SaveAsync(CountingState state)
        {
            var existing = await _context.CountingStates
                .FirstOrDefaultAsync(x => x.BotId == state.BotId && x.ChannelId == state.ChannelId);

            if (existing == null)
            {
                state.UpdatedAt = DateTime.UtcNow;
                await _context.CountingStates.AddAsync(state);
            }
            else
            {
                existing.CurrentCount = state.CurrentCount;
                existing.LastUserId = state.LastUserId;
                existing.LastMessageId = state.LastMessageId;
                existing.HighScore = state.HighScore;
                existing.BossActive = state.BossActive;
                existing.BossAnswer = state.BossAnswer;
                existing.BossSpawnedAt = state.BossSpawnedAt;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
    }
}
