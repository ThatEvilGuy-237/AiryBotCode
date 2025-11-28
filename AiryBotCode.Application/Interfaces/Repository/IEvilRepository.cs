namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface IEvilRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteAllAsync();
        public Task SaveChangesAsync();
    }
}
