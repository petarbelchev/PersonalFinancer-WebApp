namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;

	public class AccountsTypesSeeder : IUserDataSeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user)
        {
            if (await dbContext.AccountTypes.AnyAsync(at => at.OwnerId == user.Id))
                return;

            var accountTypes = new AccountType[]
            {
                new AccountType
                {
                    Id = Guid.NewGuid(),
                    Name = "Cash",
                    OwnerId = user.Id,
                },
                new AccountType
                {
                    Id = Guid.NewGuid(),
                    Name = "Bank",
                    OwnerId = user.Id,
                },
                new AccountType
                {
                    Id = Guid.NewGuid(),
                    Name = "Savings",
                    OwnerId = user.Id,
                },
            };

            await dbContext.AccountTypes.AddRangeAsync(accountTypes);
            await dbContext.SaveChangesAsync();
        }
    }
}
