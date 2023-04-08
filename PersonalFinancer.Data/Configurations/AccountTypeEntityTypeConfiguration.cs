namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	using Models;
	using static Constants.SeedConstants;

	internal class AccountTypeEntityTypeConfiguration : IEntityTypeConfiguration<AccountType>
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
				},
				new AccountType
				{
					Id = SecondUserCashMoneyAccountTypeId,
					Name = "Cash Money",
					OwnerId = SecondUserId
				},
				new AccountType
				{
					Id = SecondUserBankMoneyAccountTypeId,
					Name = "Bank Money",
					OwnerId = SecondUserId
				}
			});
		}
	}
}
