﻿namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.AspNetCore.Identity;
	using PersonalFinancer.Data.Models;
	using System.Threading.Tasks;
	using static PersonalFinancer.Common.Constants.RoleConstants;
	using static PersonalFinancer.Common.Constants.SeedConstants;

	public static class UserSeeder
	{
		public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
		{
			ApplicationUser? admin = await userManager.FindByEmailAsync(AdminEmail);

			if (admin == null)
			{
				admin = new ApplicationUser
				{
					FirstName = "Great",
					LastName = "Admin",
					Email = "admin@admin.com",
					NormalizedEmail = "ADMIN@ADMIN.COM",
					UserName = "admin",
					NormalizedUserName = "ADMIN",
					PhoneNumber = "0987654321",
					EmailConfirmed = true,
					IsAdmin = true
				};

				IdentityResult creationResult =
				   await userManager.CreateAsync(admin, AdminPassword);

				if (creationResult.Succeeded)
					await userManager.AddToRoleAsync(admin, AdminRoleName);
			}

			ApplicationUser? testUser = await userManager.FindByEmailAsync(FirstUserEmail);

			if (testUser == null)
			{
				testUser = new ApplicationUser
				{
					FirstName = "Petar",
					LastName = "Petrov",
					Email = "petar@mail.com",
					NormalizedEmail = "PETAR@MAIL.COM",
					UserName = "petar",
					NormalizedUserName = "PETAR2023",
					PhoneNumber = "1234567890",
					EmailConfirmed = true,
				};

				IdentityResult creationResult = 
					await userManager.CreateAsync(testUser, FirstUserPassword);

				if (creationResult.Succeeded)
					await userManager.AddToRoleAsync(testUser, UserRoleName);
			}			
		}
	}
}
