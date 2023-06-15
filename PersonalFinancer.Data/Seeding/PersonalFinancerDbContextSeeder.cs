namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using PersonalFinancer.Data.Models;
	using System;
	using System.Threading.Tasks;
	using static PersonalFinancer.Data.Constants;

	public class PersonalFinancerDbContextSeeder
	{
		public async Task SeedAsync(PersonalFinancerDbContext dbContext, IServiceProvider serviceProvider)
		{
			RoleManager<IdentityRole<Guid>> roleManager =
				serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
			UserManager<ApplicationUser> userManager =
			   serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			// This seed is mandatory, except for seeding the test user.
			await RoleSeeder.SeedAsync(roleManager);
			await UserSeeder.SeedAsync(userManager);
			await this.SeedInitialBalanceCategory(dbContext, userManager);

			// This seed is for the test user and is not mandatory.
			// If you don't have a test user, don't execute this seed.
			ApplicationUser testUser = await userManager.FindByEmailAsync(SeedConstants.FirstUserEmail);
			var userDataSeeders = new IUserDataSeeder[]
			{
				new AccountTypeSeeder(),
				new CurrencySeeder(),
				new AccountSeeder(),
				new CategorySeeder(),
				new TransactionSeeder(),
			};

			foreach (IUserDataSeeder seeder in userDataSeeders)
				await seeder.SeedAsync(dbContext, testUser);
		}

		private async Task SeedInitialBalanceCategory(PersonalFinancerDbContext dbContext, UserManager<ApplicationUser> userManager)
		{
			var initialBalanceCategoryId = Guid.Parse(CategoryConstants.InitialBalanceCategoryId);

			if (!await dbContext.Categories.AnyAsync(c => c.Id == initialBalanceCategoryId))
			{
				ApplicationUser admin = await userManager.FindByEmailAsync(SeedConstants.AdminEmail);

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
