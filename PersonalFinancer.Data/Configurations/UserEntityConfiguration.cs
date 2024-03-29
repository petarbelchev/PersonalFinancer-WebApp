﻿namespace PersonalFinancer.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using PersonalFinancer.Data.Models;
    using static PersonalFinancer.Common.Constants.UserConstants;

    internal class UserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
               .Property(p => p.UserName)
               .HasMaxLength(UserNameMaxLength)
               .IsRequired();

            builder
               .Property(p => p.NormalizedUserName)
               .HasMaxLength(UserNameMaxLength)
               .IsRequired();

            builder
               .Property(p => p.PhoneNumber)
               .HasMaxLength(PhoneNumberLength);

            builder
               .HasMany(a => a.Transactions)
               .WithOne(a => a.Owner)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
