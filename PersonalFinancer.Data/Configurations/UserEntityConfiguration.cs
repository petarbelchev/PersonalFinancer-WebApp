using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.Constants.UserConstants;

namespace PersonalFinancer.Data.Configurations
{
	internal class UserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder
				.Property(p => p.UserName)
				.HasMaxLength(UserNameMaxLength)
				.IsRequired();

			builder
				.HasMany(a => a.Transactions)
				.WithOne(a => a.Owner)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
