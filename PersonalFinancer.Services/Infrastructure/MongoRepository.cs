namespace PersonalFinancer.Services.Infrastructure
{
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;

	using Data;

	public class MongoRepository<T> : IMongoRepository<T> where T : class
	{
		private IMongoCollection<T> collection;

		public MongoRepository(IMongoDbContext context)
			=> collection = context.GetCollection<T>(typeof(T).Name);

		public async Task AddAsync(T entity)
			=> await collection.InsertOneAsync(entity);

		public IMongoQueryable<T> All() => collection.AsQueryable();

		public async Task Remove(FilterDefinition<T> filter)
			=> await collection.DeleteOneAsync(filter);

		public async Task UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
			=> await collection.UpdateOneAsync(filter, update);
	}
}
