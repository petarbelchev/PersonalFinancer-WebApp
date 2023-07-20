namespace PersonalFinancer.Data.Configurations
{
    public class MongoDbConfigurationSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;
    }
}
