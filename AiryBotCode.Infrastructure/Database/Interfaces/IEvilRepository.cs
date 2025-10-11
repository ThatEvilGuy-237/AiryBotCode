﻿
namespace AiryBotCode.Infrastructure.Database.Interfaces
{
    public interface IEvilRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        public Task SaveChangesAsync();
    }
}
