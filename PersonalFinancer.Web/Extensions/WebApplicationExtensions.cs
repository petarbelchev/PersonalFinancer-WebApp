namespace Microsoft.Extensions.DependencyInjection
{
	using Microsoft.AspNetCore.Identity;
	using PersonalFinancer.Data;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Seeding;
	using PersonalFinancer.Web.Hubs;

	public static class WebApplicationExtensions
	{
		public static async Task<WebApplication> SeedDatabase(this WebApplication app)
		{
			using IServiceScope scope = app.Services.CreateScope();
			IServiceProvider serviceProvider = scope.ServiceProvider;

			var dbContext = serviceProvider.GetRequiredService<PersonalFinancerDbContext>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			await PersonalFinancerDbContextSeeder.SeedAsync(dbContext, roleManager, userManager);

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
