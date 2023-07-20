namespace PersonalFinancer.Data.Repositories
{
    public interface IEfRepository<T> where T : class
    {
        Task AddAsync(T entity);

        IQueryable<T> All();

        Task<T?> FindAsync(Guid id);

        void Remove(T entity);

        Task<int> SaveChangesAsync();
    }
}
