namespace PersonalFinancer.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using PersonalFinancer.Data.Models;

    internal class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder
                .HasMany(c => c.Transactions)
                .WithOne(t => t.Category)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
