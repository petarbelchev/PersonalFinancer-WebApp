namespace PersonalFinancer.Data.Configurations
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using static PersonalFinancer.Common.Constants.RoleConstants;

    internal class UserRoleEntityConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
        {
            builder
                .Property(p => p.Name)
                .HasMaxLength(NameMaxLength)
                .IsRequired();

            builder
                .Property(p => p.NormalizedName)
                .HasMaxLength(NameMaxLength)
                .IsRequired();
        }
    }
}
