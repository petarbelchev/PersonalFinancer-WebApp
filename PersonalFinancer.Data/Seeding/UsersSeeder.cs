namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using System.Threading.Tasks;
	using static PersonalFinancer.Common.Constants.RoleConstants;
	using static PersonalFinancer.Common.Constants.SeedConstants;

	public static class UsersSeeder
	{
		public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
		{
			bool isAdminExist = await userManager.Users.AnyAsync(u => u.IsAdmin);

			if (!isAdminExist)
			{
				var admin = new ApplicationUser
				{
					FirstName = "Alfa",
					LastName = "Admin",
					Email = FirstAdminEmail,
					NormalizedEmail = FirstAdminEmail.ToUpper(),
					UserName = "alfa.admin",
					NormalizedUserName = "ALFA.ADMIN",
					PhoneNumber = "0987654321",
					EmailConfirmed = true,
					IsAdmin = true
				};

				IdentityResult creationResult =
				   await userManager.CreateAsync(admin, FirstAdminPassword);

				if (creationResult.Succeeded)
					await userManager.AddToRoleAsync(admin, AdminRoleName);
			}

			// The code below is a test data, and it's not mandatory.
			ApplicationUser? testUser = await userManager.FindByEmailAsync(FirstTestUserEmail);

			if (testUser == null)
			{
				testUser = new ApplicationUser
				{
					FirstName = "Test",
					LastName = "User",
					Email = FirstTestUserEmail,
					NormalizedEmail = FirstTestUserEmail.ToUpper(),
					UserName = "test-user",
					NormalizedUserName = "TEST-USER",
					PhoneNumber = "1234567890",
					EmailConfirmed = true,
				};

				IdentityResult creationResult =
					await userManager.CreateAsync(testUser, FirstTestUserPassword);

				if (creationResult.Succeeded)
					await userManager.AddToRoleAsync(testUser, UserRoleName);
			}

			int usersCount = await userManager.Users.CountAsync();
			int desiredUsersCount = 15;

			if (usersCount <= 10)
			{
				for (int i = usersCount; i <= desiredUsersCount; i++)
				{
					var newUser = new ApplicationUser
					{
						FirstName = "Test",
						LastName = "User",
						Email = $"test.user{i}@mail.com",
						NormalizedEmail = $"TEST.USER{i}@MAIL.COM",
						UserName = $"test-user{i}",
						NormalizedUserName = $"TEST-USER{i}",
						PhoneNumber = "1234567890",
						EmailConfirmed = true,
					};

					IdentityResult creationResult =
						await userManager.CreateAsync(newUser, $"TestUser{i}!");

					if (creationResult.Succeeded)
						await userManager.AddToRoleAsync(testUser, UserRoleName);
				}
			}
		}
	}
}
