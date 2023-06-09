namespace PersonalFinancer.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    public class PersonalFinancerDbContextSeeder : ISeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, IServiceProvider serviceProvider)
        {
            var seeders = new List<ISeeder>
            {
            new RoleSeeder(),
            new UserSeeder(),
            new AccountTypeSeeder(),
            new CurrencySeeder(),
            new AccountSeeder(),
            new CategorySeeder(),
            new TransactionSeeder(),
            };

            foreach (ISeeder seeder in seeders)
                await seeder.SeedAsync(dbContext, serviceProvider);
        }
    }
}
