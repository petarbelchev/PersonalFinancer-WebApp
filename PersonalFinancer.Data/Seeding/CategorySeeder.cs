namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using static PersonalFinancer.Common.Constants.SeedConstants;

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
                    Id = Guid.Parse(FoodDrinkCategoryId),
                    Name = "Food & Drink",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(UtilitiesCategoryId),
                    Name = "Utilities",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(TransportCategoryId),
                    Name = "Transport",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(MedicalHealthcareCategoryId),
                    Name = "Medical & Healthcare",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(SalaryCategoryId),
                    Name = "Salary",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(MoneyTransferCategoryId),
                    Name = "Money Transfer",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.Parse(DividendsCategoryId),
                    Name = "Dividends",
                    OwnerId = user.Id,
                },
            };

            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
        }
    }
}
