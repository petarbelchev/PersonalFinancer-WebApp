namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.AspNetCore.Identity;
	using System;
	using System.Threading.Tasks;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	public static class RolesSeeder
	{
		public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager)
		{
			if (!await roleManager.RoleExistsAsync(AdminRoleName))
			{
				var adminRole = new IdentityRole<Guid> { Name = AdminRoleName };
				await roleManager.CreateAsync(adminRole);
			}

			if (!await roleManager.RoleExistsAsync(UserRoleName))
			{
				var userRole = new IdentityRole<Guid> { Name = UserRoleName };
				await roleManager.CreateAsync(userRole);
			}
		}
	}
}
