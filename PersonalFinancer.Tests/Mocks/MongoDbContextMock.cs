namespace PersonalFinancer.Tests.Mocks
{
	using Microsoft.Extensions.Options;
	using PersonalFinancer.Data;

	static class MongoDbContextMock
	{
		public static MongoDbContext Instance
		{
			get
			{
				var dbSettings = new MongoDbSettings
				{
					ConnectionString = "mongodb://localhost:27017",
					DatabaseName = "MessagesMock"
				};

				IOptions<MongoDbSettings> options = Options.Create(dbSettings);
				var context = new MongoDbContext(options);

				return context;
			}
		}
	}
}
