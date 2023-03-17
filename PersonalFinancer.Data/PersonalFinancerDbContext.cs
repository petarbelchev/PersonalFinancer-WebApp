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
			else
			{
				this.Database.Migrate();
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

				b.HasMany(a => a.Accounts).WithOne(a => a.Owner).OnDelete(DeleteBehavior.NoAction);
			});

			builder.Entity<Account>(b =>
			{
				b.Property(a => a.Balance).HasColumnType("decimal").HasPrecision(18, 2);
				b.HasOne(a => a.Currency).WithMany().OnDelete(DeleteBehavior.NoAction);
			});

			builder.Entity<Transaction>(b =>
			{
				b.Property(a => a.Amount).HasColumnType("decimal").HasPrecision(18, 2);
				b.HasOne(t => t.Category).WithMany().OnDelete(DeleteBehavior.NoAction);
			});

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