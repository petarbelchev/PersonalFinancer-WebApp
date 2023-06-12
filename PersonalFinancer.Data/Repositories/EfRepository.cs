namespace PersonalFinancer.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;

    public class EfRepository<T> : IEfRepository<T> where T : class
    {
        private readonly PersonalFinancerDbContext context;
        private readonly DbSet<T> dbSet;

        public EfRepository(PersonalFinancerDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
           => await this.dbSet.AddAsync(entity);

        public IQueryable<T> All()
           => this.dbSet.AsQueryable();

        public async Task<T?> FindAsync(Guid id)
           => await this.dbSet.FindAsync(id);

        public void Remove(T entity)
           => this.dbSet.Remove(entity);

        public async Task<int> SaveChangesAsync()
           => await this.context.SaveChangesAsync();
    }
}
