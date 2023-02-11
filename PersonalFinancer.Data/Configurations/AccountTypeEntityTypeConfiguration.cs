namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	using Data.Models;

	internal class AccountTypeEntityTypeConfiguration : IEntityTypeConfiguration<AccountType>
	{
		public void Configure(EntityTypeBuilder<AccountType> builder)
		{
			builder.HasData(new AccountType[]
			{
				new AccountType
				{
					Id = 1,
					Name = "Cash"
				},
				new AccountType
				{
					Id = 2,
					Name = "Bank Account"
				}
			});
		}
	}
}
