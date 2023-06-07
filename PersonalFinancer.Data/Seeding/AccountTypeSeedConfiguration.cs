using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.Constants.SeedConstants;

namespace PersonalFinancer.Data.Seeding
{
	internal class AccountTypeSeedConfiguration : IEntityTypeConfiguration<AccountType>
	{
		public void Configure(EntityTypeBuilder<AccountType> builder)
		{
			builder.HasData(new AccountType[]
			{
				new AccountType
				{
					Id = FirstUserCashAccountTypeId,
					Name = "Cash",
					OwnerId = FirstUserId
				},
				new AccountType
				{
					Id = FirstUserBankAccountTypeId,
					Name = "Bank",
					OwnerId = FirstUserId
				},
				new AccountType
				{
					Id = FirstUserSavingAccountTypeId,
					Name = "Savings",
					OwnerId = FirstUserId
				}
			});
		}
	}
}
