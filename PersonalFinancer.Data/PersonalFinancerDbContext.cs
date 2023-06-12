namespace PersonalFinancer.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using PersonalFinancer.Data.Configurations;
    using PersonalFinancer.Data.Models;

    public class PersonalFinancerDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public PersonalFinancerDbContext(DbContextOptions<PersonalFinancerDbContext> options)
            : base(options)
        {
            if (!this.Database.IsRelational())
                _ = this.Database.EnsureCreated();
        }

        public DbSet<Account> Accounts { get; set; } = null!;

        public DbSet<AccountType> AccountTypes { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Transaction> Transactions { get; set; } = null!;

        public DbSet<Currency> Currencies { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            _ = builder.ApplyConfiguration(new UserEntityConfiguration());
            _ = builder.ApplyConfiguration(new UserRoleEntityConfiguration());
            _ = builder.ApplyConfiguration(new AccountTypeEntityConfiguration());
            _ = builder.ApplyConfiguration(new CurrencyEntityConfiguration());
            _ = builder.ApplyConfiguration(new CategoryEntityConfiguration());
        }
    }
}