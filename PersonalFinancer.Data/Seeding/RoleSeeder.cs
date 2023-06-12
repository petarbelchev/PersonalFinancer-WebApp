namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.AspNetCore.Identity;
	using System;
	using System.Threading.Tasks;
	using static PersonalFinancer.Data.Constants;

	public static class RoleSeeder
	{
		public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager)
		{
			if (!await roleManager.RoleExistsAsync(RoleConstants.AdminRoleName))
			{
				var adminRole = new IdentityRole<Guid> { Name = RoleConstants.AdminRoleName };
				_ = await roleManager.CreateAsync(adminRole);
			}

			if (!await roleManager.RoleExistsAsync(RoleConstants.UserRoleName))
			{
				var userRole = new IdentityRole<Guid> { Name = RoleConstants.UserRoleName };
				_ = await roleManager.CreateAsync(userRole);
			}
		}
	}
}
