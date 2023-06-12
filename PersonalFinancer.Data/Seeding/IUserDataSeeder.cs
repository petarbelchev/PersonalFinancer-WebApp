namespace PersonalFinancer.Data.Seeding
{
	using PersonalFinancer.Data;
	using PersonalFinancer.Data.Models;
	using System.Threading.Tasks;

	public interface IUserDataSeeder
    {
        Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user);
    }
}