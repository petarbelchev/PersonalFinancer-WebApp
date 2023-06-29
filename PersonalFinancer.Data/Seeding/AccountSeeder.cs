namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using static PersonalFinancer.Common.Constants.SeedConstants;

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
                    Id = Guid.Parse(CashBgnAccountId),
                    Name = "Cash BGN",
                    Balance = 0,
                    AccountTypeId = Guid.Parse(FirstUserCashAccountTypeId),
                    CurrencyId = Guid.Parse(FirstUserBGNCurrencyId),
                    OwnerId = user.Id,
                },
                new Account
                {
                    Id = Guid.Parse(BankBgnAccountId),
                    Name = "Bank BGN",
                    Balance = 0,
                    AccountTypeId = Guid.Parse(FirstUserBankAccountTypeId),
                    CurrencyId = Guid.Parse(FirstUserBGNCurrencyId),
                    OwnerId = user.Id,
                },
                new Account
                {
                    Id = Guid.Parse(BankEurAccountId),
                    Name = "Euro Savings",
                    Balance = 0,
                    AccountTypeId = Guid.Parse(FirstUserSavingAccountTypeId),
                    CurrencyId = Guid.Parse(FirstUserEURCurrencyId),
                    OwnerId = user.Id,
                },
                new Account
                {
                    Id = Guid.Parse(BankUsdAccountId),
                    Name = "Dollar Savings",
                    Balance = 0,
                    AccountTypeId = Guid.Parse(FirstUserSavingAccountTypeId),
                    CurrencyId = Guid.Parse(FirstUserUSDCurrencyId),
                    OwnerId = user.Id,
                },
            };

            await dbContext.Accounts.AddRangeAsync(accounts);
            await dbContext.SaveChangesAsync();
        }
    }
}
