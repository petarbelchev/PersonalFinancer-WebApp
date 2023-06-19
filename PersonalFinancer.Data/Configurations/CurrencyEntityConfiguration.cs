namespace PersonalFinancer.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using PersonalFinancer.Data.Models;

    internal class CurrencyEntityConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder
               .HasMany(c => c.Accounts)
               .WithOne(a => a.Currency)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
