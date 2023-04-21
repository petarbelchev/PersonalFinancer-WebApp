namespace PersonalFinancer.Services.Infrastructure
{
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;

	public interface IMongoRepository<T> where T : class
	{
		Task AddAsync(T entity);

		IMongoQueryable<T> All();

		Task Remove(FilterDefinition<T> filter);

		Task UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update);
	}
}
