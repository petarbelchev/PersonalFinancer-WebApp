namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using static PersonalFinancer.Data.Constants;

	public class CategorySeeder : IUserDataSeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user)
        {
			if (await dbContext.Categories.AnyAsync(c => c.OwnerId == user.Id))
				return;

			var categories = new Category[]
            {
                new Category
                {
                    Id = Guid.Parse(SeedConstants.FoodDrinkCategoryId),
                    Name = "Food & Drink",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(SeedConstants.UtilitiesCategoryId),
                    Name = "Utilities",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(SeedConstants.TransportCategoryId),
                    Name = "Transport",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(SeedConstants.MedicalHealthcareCategoryId),
                    Name = "Medical & Healthcare",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(SeedConstants.SalaryCategoryId),
                    Name = "Salary",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(SeedConstants.MoneyTransferCategoryId),
                    Name = "Money Transfer",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(SeedConstants.DividendsCategoryId),
                    Name = "Dividends",
                    OwnerId = user.Id,
                },
            };

            await dbContext.Categories.AddRangeAsync(categories);
            _ = await dbContext.SaveChangesAsync();
        }
    }
}
