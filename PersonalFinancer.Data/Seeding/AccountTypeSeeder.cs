namespace PersonalFinancer.Data.Seeding
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using PersonalFinancer.Data.Models;
    using static PersonalFinancer.Data.Constants.SeedConstants;

    public class AccountTypeSeeder : ISeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.AccountTypes.Any())
                return;

            UserManager<ApplicationUser> userManager =
               serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            ApplicationUser user = await userManager.FindByEmailAsync(FirstUserEmail);

            var accountTypes = new AccountType[]
            {
                new AccountType
                {
                    Id = Guid.Parse(FirstUserCashAccountTypeId),
                    Name = "Cash",
                    OwnerId = user.Id,
                },
                new AccountType
                {
                    Id = Guid.Parse(FirstUserBankAccountTypeId),
                    Name = "Bank",
                    OwnerId = user.Id,
                },
                new AccountType
                {
                    Id = Guid.Parse(FirstUserSavingAccountTypeId),
                    Name = "Savings",
                    OwnerId = user.Id,
                },
            };

            await dbContext.AccountTypes.AddRangeAsync(accountTypes);
            _ = await dbContext.SaveChangesAsync();
        }
    }
}
