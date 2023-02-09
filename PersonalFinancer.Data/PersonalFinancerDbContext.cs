using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalFinancer.Data.Configurations;
using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Web.Data
{
	public class PersonalFinancerDbContext : IdentityDbContext<ApplicationUser>
	{
		public PersonalFinancerDbContext(DbContextOptions<PersonalFinancerDbContext> options)
			: base(options)
		{
		}

		public DbSet<Account> Accounts { get; set; } = null!;

		public DbSet<AccountType> AccountTypes { get; set; } = null!;

		public DbSet<Category> Categories { get; set; } = null!;

		public DbSet<Transaction> Transactions { get; set; } = null!;

		public DbSet<Currency> Currencies { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ApplicationUser>(b =>
			{
				b.Property(p => p.UserName).HasMaxLength(50);
				b.Property(p => p.NormalizedUserName).HasMaxLength(50);
			});

			builder.ApplyConfiguration(new AccountTypeEntityTypeConfiguration());
			builder.ApplyConfiguration(new CurrencyTypeEntityTypeConfiguration());
			builder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
		}
	}
}