namespace PersonalFinancer.Data
{
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;

	using Configurations;
	using Models;

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

			//builder.Entity<Account>(b =>
			//{
			//	b
			//		.HasOne(p => p.AccountType)
			//		.WithMany()
			//		.HasForeignKey(p => p.AccountTypeId)
			//		.OnDelete(DeleteBehavior.Restrict);

			//	b
			//		.HasOne(p => p.Currency)
			//		.WithMany()
			//		.HasForeignKey(p => p.CurrencyId)
			//		.OnDelete(DeleteBehavior.Restrict);
			//});

			//builder.Entity<Transaction>(b =>
			//{
			//	b
			//		.HasOne(p => p.Account)
			//		.WithMany()
			//		.HasForeignKey(p => p.AccountId) 
			//		.OnDelete(DeleteBehavior.Restrict);

			//	b
			//		.HasOne(p => p.Category)
			//		.WithMany()
			//		.HasForeignKey(p => p.CategoryId)
			//		.OnDelete(DeleteBehavior.Restrict);
			//});

			builder.ApplyConfiguration(new AccountTypeEntityTypeConfiguration());
			builder.ApplyConfiguration(new CurrencyTypeEntityTypeConfiguration());
			builder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
		}
	}
}