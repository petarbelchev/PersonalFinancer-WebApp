using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using PersonalFinancer.Data.Configurations;
using PersonalFinancer.Data.Models.Contracts;

namespace PersonalFinancer.Data
{
	public class MessagesDbContext : IMongoDbContext
	{
		private readonly IMongoClient client;
		private readonly IMongoDatabase database;

		public MessagesDbContext(IOptions<MongoDbSettings> settings)
		{
			var camelCaseConvension = new ConventionPack { new CamelCaseElementNameConvention() };
			ConventionRegistry.Register("camelCase", camelCaseConvension, type => true);

			client = new MongoClient(settings.Value.ConnectionString);

			database = client.GetDatabase(settings.Value.DatabaseName);
		}

		public IMongoCollection<T> GetCollection<T>(string name)
			=> database.GetCollection<T>(name);
	}
}
