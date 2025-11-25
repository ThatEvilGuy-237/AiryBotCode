using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class EvilRepository<T> : IEvilRepository<T> where T : class
    {
        protected readonly AIDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public EvilRepository(AIDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAllAsync()
        {
            var allEntities = await _dbSet.ToListAsync();
            _dbSet.RemoveRange(allEntities);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
