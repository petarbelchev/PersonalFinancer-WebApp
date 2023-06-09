namespace PersonalFinancer.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using PersonalFinancer.Data.Models;

    internal class AccountTypeEntityConfiguration : IEntityTypeConfiguration<AccountType>
    {
        public void Configure(EntityTypeBuilder<AccountType> builder)
        {
            _ = builder
               .HasMany(a => a.Accounts)
               .WithOne(a => a.AccountType)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
