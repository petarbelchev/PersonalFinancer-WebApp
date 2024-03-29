﻿namespace PersonalFinancer.Data
{
    using Microsoft.Extensions.Options;
    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Driver;
    using PersonalFinancer.Data.Configurations;

    public class MessagesDbContext : IMongoDbContext
    {
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;

        public MessagesDbContext(IOptions<MongoDbConfigurationSettings> settings)
        {
            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", camelCaseConvention, type => true);

            this.client = new MongoClient(settings.Value.ConnectionString);
            this.database = this.client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
           => this.database.GetCollection<T>(name);
    }
}
