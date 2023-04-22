namespace PersonalFinancer.Data.Repositories
{
	using Microsoft.EntityFrameworkCore;

	using Data;

	public class EfRepository<T> : IEfRepository<T> where T : class
	{
		private readonly SqlDbContext context;
		private readonly DbSet<T> dbSet;

		public EfRepository(SqlDbContext context)
		{
			this.context = context;
			this.dbSet = context.Set<T>();
		}

		public async Task AddAsync(T entity)
			=> await dbSet.AddAsync(entity);

		public IQueryable<T> All()
			=> dbSet.AsQueryable();

		public async Task<T?> FindAsync(string id)
			=> await dbSet.FindAsync(id);

		public void Remove(T entity)
			=> dbSet.Remove(entity);

		public async Task<int> SaveChangesAsync()
			=> await context.SaveChangesAsync();
	}
}
