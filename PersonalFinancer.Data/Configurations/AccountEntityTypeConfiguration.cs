using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.Constants.SeedConstants;

namespace PersonalFinancer.Data.Configurations
{
	internal class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>
	{
		public void Configure(EntityTypeBuilder<Account> builder)
		{
			builder.HasData(SeedAccounts());
		}

		private IEnumerable<Account> SeedAccounts()
		{
			var accounts = new List<Account>()
			{
				new Account
				{
					Id = Guid.Parse(CashBgnAccountId),
					Name = "Cash BGN",
					AccountTypeId = Guid.Parse(CashAccountTypeId),
					Balance = 2000,
					CurrencyId = Guid.Parse(BgnCurrencyId),
					OwnerId = FirstUserId
				},
				new Account
				{
					Id = Guid.Parse(BankBgnAccountId),
					Name = "Bank BGN",
					AccountTypeId = Guid.Parse(BankAccountTypeId),
					Balance = 4000,
					CurrencyId = Guid.Parse(BgnCurrencyId),
					OwnerId = FirstUserId
				},
				new Account
				{
					Id = Guid.Parse(BankEurAccountId),
					Name = "Euro Savings",
					AccountTypeId = Guid.Parse(SavingAccountTypeId),
					Balance = 2800,
					CurrencyId = Guid.Parse(EurCurrencyId),
					OwnerId = FirstUserId
				},
				new Account
				{
					Id = Guid.Parse(BankUsdAccountId),
					Name = "Dolar Savings",
					AccountTypeId = Guid.Parse(SavingAccountTypeId),
					Balance = 3800,
					CurrencyId = Guid.Parse(UsdCurrencyId),
					OwnerId = FirstUserId
				}
			};

			return accounts;
		}
	}
}
