namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using static PersonalFinancer.Common.Constants.SeedConstants;

	public class AccountTypeSeeder : IUserDataSeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user)
        {
            if (await dbContext.AccountTypes.AnyAsync(at => at.OwnerId == user.Id))
                return;

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
            await dbContext.SaveChangesAsync();
        }
    }
}
