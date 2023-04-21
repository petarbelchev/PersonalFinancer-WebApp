namespace PersonalFinancer.Data
{
	using MongoDB.Driver;

	public interface IMongoDbContext
	{
		IMongoCollection<T> GetCollection<T>(string name);

		Task DropDatabaseAsync(string name);
	}
}
