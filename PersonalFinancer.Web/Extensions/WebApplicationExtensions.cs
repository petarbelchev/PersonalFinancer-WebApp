namespace PersonalFinancer.Web.Extensions
{
	using PersonalFinancer.Data;
	using PersonalFinancer.Data.Seeding;
	using PersonalFinancer.Web.Hubs;

	public static class WebApplicationExtensions
	{
		public static WebApplication SeedDatabase(this WebApplication app)
		{
			using IServiceScope scope = app.Services.CreateScope();
			IServiceProvider serviceProvider = scope.ServiceProvider;

			PersonalFinancerDbContext dbContext =
				serviceProvider.GetRequiredService<PersonalFinancerDbContext>();

			PersonalFinancerDbContextSeeder
				.SeedAsync(dbContext, serviceProvider)
				.GetAwaiter()
				.GetResult();

			return app;
		}

		public static WebApplication UseSignalRHubs(this WebApplication app)
		{
			app.MapHub<MessageHub>("/message");
			app.MapHub<NotificationsHub>("/notifications");
			app.MapHub<AllMessagesHub>("/allMessages");

			return app;
		}
	}
}
