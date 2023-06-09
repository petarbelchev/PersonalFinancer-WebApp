namespace PersonalFinancer.Data.Seeding
{
    using PersonalFinancer.Data;
    using System;
    using System.Threading.Tasks;

    public interface ISeeder
    {
        Task SeedAsync(PersonalFinancerDbContext dbContext, IServiceProvider serviceProvider);
    }
}