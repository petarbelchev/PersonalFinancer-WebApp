namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;

	public class CategoriesSeeder : IUserDataSeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user)
        {
			if (await dbContext.Categories.AnyAsync(c => c.OwnerId == user.Id))
				return;

			var categories = new Category[]
            {
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Food & Drink",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Utilities",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Transport",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Medical & Healthcare",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Money Transfer",
                    OwnerId = user.Id,
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Dividends",
                    OwnerId = user.Id,
                },
            };

            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
        }
    }
}
