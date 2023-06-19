namespace PersonalFinancer.Web.Extensions
{
	using PersonalFinancer.Data;
	using PersonalFinancer.Data.Seeding;

	public static class WebApplicationExtensions
	{
		public static WebApplication SeedDatabase(this WebApplication app)
		{
			using IServiceScope scope = app.Services.CreateScope();
			IServiceProvider serviceProvider = scope.ServiceProvider;

			PersonalFinancerDbContext dbContext =
				serviceProvider.GetRequiredService<PersonalFinancerDbContext>();

			new PersonalFinancerDbContextSeeder()
				.SeedAsync(dbContext, serviceProvider)
				.GetAwaiter()
				.GetResult();

			return app;
		}
	}
}
