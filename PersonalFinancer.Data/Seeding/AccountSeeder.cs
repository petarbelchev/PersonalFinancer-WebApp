namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using static PersonalFinancer.Data.Constants;

	public class AccountSeeder : IUserDataSeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user)
        {
            if (await dbContext.Accounts.AnyAsync(a => a.OwnerId == user.Id))
                return;

            var accounts = new Account[]
            {
                new Account
                {
                    Id = Guid.Parse(SeedConstants.CashBgnAccountId),
                    Name = "Cash BGN",
                    Balance = 0,
                    AccountTypeId = Guid.Parse(SeedConstants.FirstUserCashAccountTypeId),
                    CurrencyId = Guid.Parse(SeedConstants.FirstUserBGNCurrencyId),
                    OwnerId = user.Id,
                },
                new Account
                {
                    Id = Guid.Parse(SeedConstants.BankBgnAccountId),
                    Name = "Bank BGN",
                    Balance = 0,
                    AccountTypeId = Guid.Parse(SeedConstants.FirstUserBankAccountTypeId),
                    CurrencyId = Guid.Parse(SeedConstants.FirstUserBGNCurrencyId),
                    OwnerId = user.Id,
                },
                new Account
                {
                    Id = Guid.Parse(SeedConstants.BankEurAccountId),
                    Name = "Euro Savings",
                    Balance = 0,
                    AccountTypeId = Guid.Parse(SeedConstants.FirstUserSavingAccountTypeId),
                    CurrencyId = Guid.Parse(SeedConstants.FirstUserEURCurrencyId),
                    OwnerId = user.Id,
                },
                new Account
                {
                    Id = Guid.Parse(SeedConstants.BankUsdAccountId),
                    Name = "Dolar Savings",
                    Balance = 0,
                    AccountTypeId = Guid.Parse(SeedConstants.FirstUserSavingAccountTypeId),
                    CurrencyId = Guid.Parse(SeedConstants.FirstUserUSDCurrencyId),
                    OwnerId = user.Id,
                },
            };

            await dbContext.Accounts.AddRangeAsync(accounts);
            _ = await dbContext.SaveChangesAsync();
        }
    }
}
