namespace PersonalFinancer.Data.Models.Contracts
{
    using MongoDB.Driver;

    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
