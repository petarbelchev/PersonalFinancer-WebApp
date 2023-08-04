namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;

	public class AccountsSeeder : IUserDataSeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user)
        {
            if (await dbContext.Accounts.AnyAsync(a => a.OwnerId == user.Id))
                return;

			Guid[] userAccountTypes = await dbContext.AccountTypes
				.Where(at => at.OwnerId == user.Id)
				.OrderBy(at => at.Name)
				.Select(at => at.Id)
				.ToArrayAsync();

			Guid bankAccType = userAccountTypes[0];
			Guid cashAccType = userAccountTypes[1];
			Guid savingsAccType = userAccountTypes[2];

			Guid[] userCurrencies = await dbContext.Currencies
				.Where(c => c.OwnerId == user.Id)
				.OrderBy(c => c.Name)
				.Select(c => c.Id)
				.ToArrayAsync();

			Guid bgnId = userCurrencies[0];
			Guid eurId = userCurrencies[1];
			Guid usdId = userCurrencies[2];

			var accounts = new Account[]
            {
                new Account
                {
                    Id = Guid.NewGuid(),
                    Name = "Cash BGN",
                    Balance = 0,
                    AccountTypeId = cashAccType,
                    CurrencyId = bgnId,
                    OwnerId = user.Id,
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    Name = "Bank BGN",
                    Balance = 0,
                    AccountTypeId = bankAccType,
                    CurrencyId = bgnId,
                    OwnerId = user.Id,
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    Name = "Euro Savings",
                    Balance = 0,
                    AccountTypeId = savingsAccType,
                    CurrencyId = eurId,
                    OwnerId = user.Id,
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    Name = "Dollar Savings",
                    Balance = 0,
                    AccountTypeId = savingsAccType,
                    CurrencyId = usdId,
                    OwnerId = user.Id,
                },
            };

            await dbContext.Accounts.AddRangeAsync(accounts);
            await dbContext.SaveChangesAsync();
        }
    }
}
