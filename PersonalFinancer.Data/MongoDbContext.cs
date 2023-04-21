namespace PersonalFinancer.Data
{
	using Microsoft.Extensions.Options;
	
	using MongoDB.Bson.Serialization.Conventions;
	using MongoDB.Driver;

	public class MongoDbContext : IMongoDbContext
	{
		private IMongoClient client;
		private IMongoDatabase database;

		public MongoDbContext(IOptions<MongoDbSettings> settings)
		{
			var camelCaseConvension = new ConventionPack { new CamelCaseElementNameConvention() };
			ConventionRegistry.Register("camelCase", camelCaseConvension, type => true);

			client = new MongoClient(settings.Value.ConnectionString);

			database = client.GetDatabase(settings.Value.DatabaseName);
		}

		public IMongoCollection<T> GetCollection<T>(string name)
			=> database.GetCollection<T>(name);

		public async Task DropDatabaseAsync(string name)
			=> await client.DropDatabaseAsync(name);
	}
}
