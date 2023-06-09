namespace PersonalFinancer.Data.Seeding
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using PersonalFinancer.Data.Models;
    using static PersonalFinancer.Data.Constants;

    public class CategorySeeder : ISeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any(c => c.Id != Guid.Parse(CategoryConstants.InitialBalanceCategoryId)))
                return;

            UserManager<ApplicationUser> userManager =
               serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            ApplicationUser user = await userManager.FindByEmailAsync(SeedConstants.FirstUserEmail);

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
                    Id = Guid.Parse(SeedConstants.DividentsCategoryId),
                    Name = "Dividents",
                    OwnerId = user.Id,
                },
            };

            await dbContext.Categories.AddRangeAsync(categories);
            _ = await dbContext.SaveChangesAsync();
        }
    }
}
