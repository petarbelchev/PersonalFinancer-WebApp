namespace PersonalFinancer.Web.Infrastructure
{
	using Microsoft.AspNetCore.Identity;

	using Data.Models;
	using static Data.DataConstants.RoleConstants;

	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder SeedAdmin(this IApplicationBuilder app)
		{
			using IServiceScope scope = app.ApplicationServices.CreateScope();

			IServiceProvider services = scope.ServiceProvider;
			UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
			RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

			Task.Run(async () =>
			{
				if (await roleManager.RoleExistsAsync(AdminRoleName))
				{
					return;
				}

				IdentityRole role = new IdentityRole { Name = AdminRoleName };
				await roleManager.CreateAsync(role);

				ApplicationUser admin = await userManager.FindByIdAsync("dea12856-c198-4129-b3f3-b893d8395082");
				await userManager.AddToRoleAsync(admin, AdminRoleName);
			})
				.GetAwaiter()
				.GetResult();

			return app;
		}
	}
}