namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;

	public class CurrenciesSeeder : IUserDataSeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user)
        {
			if (await dbContext.Currencies.AnyAsync(c => c.OwnerId == user.Id))
				return;

			var currencies = new Currency[]
            {
                new Currency
                {
                    Id = Guid.NewGuid(),
                    Name = "BGN",
                    OwnerId = user.Id,
                },
                new Currency
                {
                    Id = Guid.NewGuid(),
                    Name = "EUR",
                    OwnerId = user.Id,
                },
                new Currency
                {
                    Id = Guid.NewGuid(),
                    Name = "USD",
                    OwnerId = user.Id,
                },
            };

            await dbContext.Currencies.AddRangeAsync(currencies);
            await dbContext.SaveChangesAsync();
        }
    }
}
