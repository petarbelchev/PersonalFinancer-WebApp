using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Data.Configurations
{
	internal class AccountTypeEntityConfiguration : IEntityTypeConfiguration<AccountType>
	{
		public void Configure(EntityTypeBuilder<AccountType> builder)
		{
			builder
				.HasMany(a => a.Accounts)
				.WithOne(a => a.AccountType)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
