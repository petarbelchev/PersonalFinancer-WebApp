using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Data.Configurations
{
	internal class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>
	{
		public void Configure(EntityTypeBuilder<Account> builder)
		{
			builder.HasData(new Account[]
			{
				new Account
				{
					Id = 1,
					Name = "MyCashMoney",
					OwnerId = "2e8ce625-1278-4368-87c5-9c79fd7692a4",
					CurrencyId = 1,
					AccountTypeId = 1,
					Balance = 100.00m
				},
				new Account
				{
					Id = 2,
					Name = "MyBankMoney",
					OwnerId = "2e8ce625-1278-4368-87c5-9c79fd7692a4",
					CurrencyId = 2,
					AccountTypeId = 2,
					Balance = 1500.00m
				}
			});
		}
	}
}
