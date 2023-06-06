using MongoDB.Driver;

namespace PersonalFinancer.Data.Models.Contracts
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
