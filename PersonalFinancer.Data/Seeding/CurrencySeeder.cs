namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using static PersonalFinancer.Data.Constants.SeedConstants;

	public class CurrencySeeder : IUserDataSeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user)
        {
			if (await dbContext.Currencies.AnyAsync(c => c.OwnerId == user.Id))
				return;

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
