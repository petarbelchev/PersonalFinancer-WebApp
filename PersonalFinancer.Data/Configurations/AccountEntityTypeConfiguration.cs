using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PersonalFinancer.Data.Models;

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
					Id = Guid.Parse("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
					Name = "Cash BGN",
					AccountTypeId = Guid.Parse("f4c3803a-7ed5-4d78-9038-7b21bf08a040"),
					Balance = 189.55m,
					CurrencyId = Guid.Parse("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"),
					OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
				},
				new Account
				{
					Id = Guid.Parse("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
					Name = "Bank BGN",
					AccountTypeId = Guid.Parse("1dfe1780-daed-4198-8360-378aa33c5411"),
					Balance = 2734.78m,
					CurrencyId = Guid.Parse("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"),
					OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
				},
				new Account
				{
					Id = Guid.Parse("70169197-5c32-4430-ab39-34c776533376"),
					Name = "Cash EUR",
					AccountTypeId = Guid.Parse("f4c3803a-7ed5-4d78-9038-7b21bf08a040"),
					Balance = 825.71m,
					CurrencyId = Guid.Parse("dab2761d-acb1-43bc-b56b-0d9c241c8882"),
					OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
				},
				new Account
				{
					Id = Guid.Parse("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
					Name = "Bank EUR",
					AccountTypeId = Guid.Parse("1dfe1780-daed-4198-8360-378aa33c5411"),
					Balance = 900.01m,
					CurrencyId = Guid.Parse("dab2761d-acb1-43bc-b56b-0d9c241c8882"),
					OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
				},
				new Account
				{
					Id = Guid.Parse("303430dc-63a3-4436-8907-a274ec29f608"),
					Name = "Bank USD",
					AccountTypeId = Guid.Parse("1dfe1780-daed-4198-8360-378aa33c5411"),
					Balance = 1487.23m,
					CurrencyId = Guid.Parse("2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f"),
					OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
				}
			};

			return accounts;
		}
	}
}
