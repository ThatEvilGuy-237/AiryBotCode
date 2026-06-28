using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class SuggestionShareCodeRepository : ISuggestionShareCodeRepository
    {
        private readonly AIDbContext _context;

        public SuggestionShareCodeRepository(AIDbContext context) => _context = context;

        public Task<SuggestionShareCode?> GetAsync() =>
            _context.SuggestionShareCodes.AsNoTracking()
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();

        public async Task SetAsync(string code)
        {
            // Single-row store: clear any existing rows, then insert the new code.
            var existing = await _context.SuggestionShareCodes.ToListAsync();
            if (existing.Count > 0) _context.SuggestionShareCodes.RemoveRange(existing);

            await _context.SuggestionShareCodes.AddAsync(new SuggestionShareCode
            {
                Code = code,
                CreatedAt = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();
        }
    }
}
