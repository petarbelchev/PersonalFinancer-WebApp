namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Common.Constants;
	using PersonalFinancer.Data.Models;
	using System;
	using System.Threading.Tasks;

	public static class PersonalFinancerDbContextSeeder
	{
		public static async Task SeedAsync(
			PersonalFinancerDbContext dbContext, 
			RoleManager<IdentityRole<Guid>> roleManager,
			UserManager<ApplicationUser> userManager)
		{
			// This seed is mandatory, except for seeding the test users.
			await RolesSeeder.SeedAsync(roleManager);
			await UsersSeeder.SeedAsync(userManager);
			await SeedInitialBalanceCategory(dbContext, userManager);

			// This seed is for the first test user and is not mandatory.
			// If you don't have a test user, don't execute this seed.
			ApplicationUser testUser = await userManager.FindByEmailAsync(SeedConstants.FirstTestUserEmail);
			var userDataSeeders = new IUserDataSeeder[]
			{
				new AccountsTypesSeeder(),
				new CurrenciesSeeder(),
				new AccountsSeeder(),
				new CategoriesSeeder(),
				new TransactionsSeeder(),
			};

			foreach (IUserDataSeeder seeder in userDataSeeders)
				await seeder.SeedAsync(dbContext, testUser);

			// This seed is for the rest of the test users and is not mandatory.
			// If you don't have a test users, don't execute this seed.
			ApplicationUser[] testUsers = await userManager.Users
				.Where(u => !u.IsAdmin && !u.Accounts.Any())
				.ToArrayAsync();

			userDataSeeders = new IUserDataSeeder[]
			{
				new AccountsTypesSeeder(),
				new CurrenciesSeeder(),
				new AccountsSeeder()
			};

			foreach (var user in testUsers)
			{
				foreach (IUserDataSeeder seeder in userDataSeeders)
					await seeder.SeedAsync(dbContext, user);
			}
		}

		private static async Task SeedInitialBalanceCategory(
			PersonalFinancerDbContext dbContext, 
			UserManager<ApplicationUser> userManager)
		{
			var initialBalanceCategoryId = Guid.Parse(CategoryConstants.InitialBalanceCategoryId);

			if (!await dbContext.Categories.AnyAsync(c => c.Id == initialBalanceCategoryId))
			{
				ApplicationUser admin = await userManager.FindByEmailAsync(SeedConstants.FirstAdminEmail);

				await dbContext.Categories.AddAsync(new Category
				{
					Id = initialBalanceCategoryId,
					Name = CategoryConstants.CategoryInitialBalanceName,
					OwnerId = admin.Id
				});
			}
		}
	}
}
