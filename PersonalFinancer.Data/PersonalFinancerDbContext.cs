using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using PersonalFinancer.Data.Configurations;
using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Data
{
	public class PersonalFinancerDbContext : IdentityDbContext<ApplicationUser>
	{
		private bool seed;

		public PersonalFinancerDbContext(DbContextOptions<PersonalFinancerDbContext> options, bool seed = true)
			: base(options)
		{
			if (!this.Database.IsRelational())
			{
				this.Database.EnsureCreated();
			}

			this.seed = seed;
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
				b.Ignore(p => p.AccessFailedCount);
				b.Ignore(p => p.EmailConfirmed);
				b.Ignore(p => p.TwoFactorEnabled);
				b.Ignore(p => p.PhoneNumberConfirmed);
				b.Ignore(p => p.LockoutEnabled);
				b.Ignore(p => p.LockoutEnd);
			});

			builder.Entity<Account>()
				.Property(a => a.Balance)
				.HasColumnType("decimal")
				.HasPrecision(18, 2);

			builder.Entity<Transaction>()
				.Property(a => a.Amount)
				.HasColumnType("decimal")
				.HasPrecision(18, 2);

			if (this.seed)
			{
				builder.ApplyConfiguration(new UserEntityTypeConfiguration());
				builder.ApplyConfiguration(new AccountTypeEntityTypeConfiguration());
				builder.ApplyConfiguration(new CurrencyTypeEntityTypeConfiguration());
				builder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
			}
		}
	}
}