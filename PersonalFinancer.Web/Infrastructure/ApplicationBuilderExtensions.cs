namespace PersonalFinancer.Web.Infrastructure
{
	using Microsoft.AspNetCore.Identity;

	using Data.Models;
	using static Data.Constants.RoleConstants;

	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder SeedUsers(this IApplicationBuilder app)
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

				IdentityRole adminRole = new IdentityRole { Name = AdminRoleName };
				IdentityRole userRole = new IdentityRole { Name = UserRoleName };
				await roleManager.CreateAsync(adminRole);
				await roleManager.CreateAsync(userRole);

				ApplicationUser admin = await userManager.FindByIdAsync("dea12856-c198-4129-b3f3-b893d8395082");
				await userManager.AddToRoleAsync(admin, AdminRoleName);

				ApplicationUser user1 = await userManager.FindByIdAsync("6d5800ce-d726-4fc8-83d9-d6b3ac1f591e");
				ApplicationUser user2 = await userManager.FindByIdAsync("bcb4f072-ecca-43c9-ab26-c060c6f364e4");
				await userManager.AddToRoleAsync(user1, UserRoleName);
				await userManager.AddToRoleAsync(user2, UserRoleName);
			})
				.GetAwaiter()
				.GetResult();

			return app;
		}
	}
}