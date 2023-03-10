using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.Constants.SeedConstants;

namespace PersonalFinancer.Data.Configurations
{
	internal class AccountTypeEntityTypeConfiguration : IEntityTypeConfiguration<AccountType>
	{
		public void Configure(EntityTypeBuilder<AccountType> builder)
		{
			builder.HasData(new AccountType[]
			{
				new AccountType
				{
					Id = Guid.Parse(CashAccountTypeId),
					Name = "Cash",
					UserId = FirstUserId
				},
				new AccountType
				{
					Id = Guid.Parse(BankAccountTypeId),
					Name = "Bank",
					UserId = FirstUserId
				},
				new AccountType
				{
					Id = Guid.Parse(SavingAccountTypeId),
					Name = "Savings",
					UserId = FirstUserId
				}
			});
		}
	}
}
