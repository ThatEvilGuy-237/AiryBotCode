using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class UserConsentRepository : IUserConsentRepository
    {
        private readonly AIDbContext _context;

        public UserConsentRepository(AIDbContext context)
        {
            _context = context;
        }

        public Task<bool> HasConsentAsync(ulong botId, ulong userId) =>
            _context.UserConsents.AsNoTracking().AnyAsync(c => c.BotId == botId && c.UserId == userId);

        public async Task GrantAsync(ulong botId, ulong userId)
        {
            if (await _context.UserConsents.AnyAsync(c => c.BotId == botId && c.UserId == userId))
                return;   // already consented — idempotent
            await _context.UserConsents.AddAsync(new UserConsent
            {
                BotId = botId,
                UserId = userId,
                AcceptedAt = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();
        }
    }
}
