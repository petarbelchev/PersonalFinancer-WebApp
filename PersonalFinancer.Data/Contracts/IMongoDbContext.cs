using MongoDB.Driver;

namespace PersonalFinancer.Data.Contracts
{
	public interface IMongoDbContext
	{
		IMongoCollection<T> GetCollection<T>(string name);
	}
}
