using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class SuggestionRepository : ISuggestionRepository
    {
        private readonly AIDbContext _context;

        public SuggestionRepository(AIDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Suggestion>> GetAllAsync() =>
            await _context.Suggestions.AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

        public Task<Suggestion?> GetAsync(int id) =>
            _context.Suggestions.FirstOrDefaultAsync(s => s.Id == id);

        public async Task<Suggestion> AddAsync(Suggestion suggestion)
        {
            await _context.Suggestions.AddAsync(suggestion);
            await _context.SaveChangesAsync();
            return suggestion;
        }

        public async Task UpdateAsync(Suggestion suggestion)
        {
            _context.Suggestions.Update(suggestion);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Suggestions.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null) return false;
            _context.Suggestions.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Suggestion>> GetUnansweredAsync() =>
            await _context.Suggestions.AsNoTracking()
                .Where(s => s.ResponseWhy == null || s.ResponseWhy == "")
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
    }
}
