namespace PersonalFinancer.Data.Seeding
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using PersonalFinancer.Data.Models;
    using static PersonalFinancer.Data.Constants.SeedConstants;

    public class CurrencySeeder : ISeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Currencies.Any())
                return;

            UserManager<ApplicationUser> userManager =
               serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            ApplicationUser user = await userManager.FindByEmailAsync(FirstUserEmail);

            var currencies = new Currency[]
            {
                new Currency
                {
                    Id = Guid.Parse(FirstUserBGNCurrencyId),
                    Name = "BGN",
                    OwnerId = user.Id,
                },
                new Currency
                {
                    Id = Guid.Parse(FirstUserEURCurrencyId),
                    Name = "EUR",
                    OwnerId = user.Id,
                },
                new Currency
                {
                    Id = Guid.Parse(FirstUserUSDCurrencyId),
                    Name = "USD",
                    OwnerId = user.Id,
                },
            };

            await dbContext.Currencies.AddRangeAsync(currencies);
            _ = await dbContext.SaveChangesAsync();
        }
    }
}
