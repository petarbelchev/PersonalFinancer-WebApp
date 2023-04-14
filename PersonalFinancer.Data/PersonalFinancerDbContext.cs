namespace PersonalFinancer.Data
{
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;

	using Data.Configurations;
	using Data.Models;

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

				b.HasMany(a => a.Transactions).WithOne(a => a.Owner).OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Currency>(b =>
			{
				b.HasMany(c => c.Accounts).WithOne(a => a.Currency).OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<AccountType>(b =>
			{
				b.HasMany(a => a.Accounts).WithOne(a => a.AccountType).OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Category>(b =>
			{
				b.HasMany(c => c.Transactions).WithOne(t => t.Category).OnDelete(DeleteBehavior.Restrict);
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