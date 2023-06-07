using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalFinancer.Data.Configurations;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Data.Seeding;

namespace PersonalFinancer.Data
{
	public class PersonalFinancerDbContext : IdentityDbContext<ApplicationUser>
	{
		private readonly bool shouldSeedDb;

		public PersonalFinancerDbContext(
			DbContextOptions<PersonalFinancerDbContext> options, 
			bool shouldSeedDb = true)
			: base(options)
		{
			if (!this.Database.IsRelational())
			{
				this.Database.EnsureCreated();
			}

			this.shouldSeedDb = shouldSeedDb;
		}

		public DbSet<Account> Accounts { get; set; } = null!;

		public DbSet<AccountType> AccountTypes { get; set; } = null!;

		public DbSet<Category> Categories { get; set; } = null!;

		public DbSet<Transaction> Transactions { get; set; } = null!;

		public DbSet<Currency> Currencies { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfiguration(new UserEntityConfiguration());
			builder.ApplyConfiguration(new AccountTypeEntityConfiguration());
			builder.ApplyConfiguration(new CurrencyEntityConfiguration());
			builder.ApplyConfiguration(new CategoryEntityConfiguration());

			if (shouldSeedDb)
			{
				builder.ApplyConfiguration(new UsersSeedConfiguration());
				builder.ApplyConfiguration(new AccountTypeSeedConfiguration());
				builder.ApplyConfiguration(new CurrencySeedConfiguration());
				builder.ApplyConfiguration(new CategorySeedConfiguration());
			}
		}
	}
}