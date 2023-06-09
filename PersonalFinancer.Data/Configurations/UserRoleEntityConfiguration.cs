namespace PersonalFinancer.Data.Configurations
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    internal class UserRoleEntityConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
        {
            _ = builder
                .Property(p => p.Name)
                .HasMaxLength(NameMaxLength)
                .IsRequired();

            _ = builder
                .Property(p => p.NormalizedName)
                .HasMaxLength(NameMaxLength)
                .IsRequired();
        }
    }
}
