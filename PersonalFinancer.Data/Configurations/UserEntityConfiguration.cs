using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Data.Configurations
{
	internal class UserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder
				.HasMany(a => a.Transactions)
				.WithOne(a => a.Owner)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
