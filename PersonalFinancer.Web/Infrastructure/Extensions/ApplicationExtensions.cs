namespace PersonalFinancer.Web.Infrastructure.Extensions
{
	using PersonalFinancer.Data.Seeding;
	using PersonalFinancer.Data;

	public static class ApplicationExtensions
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
