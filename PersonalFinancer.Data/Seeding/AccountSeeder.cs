namespace PersonalFinancer.Data.Seeding
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using PersonalFinancer.Data.Models;
    using static PersonalFinancer.Data.Constants;

    public class AccountSeeder : ISeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Accounts.Any())
                return;

            UserManager<ApplicationUser> userManager =
               serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            ApplicationUser user = await userManager.FindByEmailAsync(SeedConstants.FirstUserEmail);

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
