using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class LevelUserRepository : ILevelUserRepository
    {
        private readonly AIDbContext _context;

        public LevelUserRepository(AIDbContext context)
        {
            _context = context;
        }

        public Task<LevelUser?> GetAsync(ulong botId, ulong userId) =>
            _context.LevelUsers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.BotId == botId && x.UserId == userId);

        public async Task SaveAsync(LevelUser user)
        {
            var existing = await _context.LevelUsers
                .FirstOrDefaultAsync(x => x.BotId == user.BotId && x.UserId == user.UserId);

            if (existing == null)
            {
                await _context.LevelUsers.AddAsync(user);
            }
            else
            {
                existing.Xp = user.Xp;
                existing.Level = user.Level;
                existing.UserName = user.UserName;
                existing.LastXpAt = user.LastXpAt;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<LevelUser>> TopAsync(ulong botId, int skip, int take) =>
            await _context.LevelUsers.AsNoTracking()
                .Where(x => x.BotId == botId)
                .OrderByDescending(x => x.Xp)
                .Skip(skip).Take(take)
                .ToListAsync();

        public Task<int> CountAsync(ulong botId) =>
            _context.LevelUsers.AsNoTracking().CountAsync(x => x.BotId == botId);

        public async Task<int> GetRankAsync(ulong botId, ulong userId)
        {
            var row = await _context.LevelUsers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.BotId == botId && x.UserId == userId);
            if (row == null) return 0;

            var ahead = await _context.LevelUsers.AsNoTracking()
                .CountAsync(x => x.BotId == botId && x.Xp > row.Xp);
            return ahead + 1;
        }
    }
}
