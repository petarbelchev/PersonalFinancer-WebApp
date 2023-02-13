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
					Id = Guid.Parse("f4c3803a-7ed5-4d78-9038-7b21bf08a040"),
					Name = "Cash"
				},
				new AccountType
				{
					Id = Guid.Parse("1dfe1780-daed-4198-8360-378aa33c5411"),
					Name = "Bank Account"
				}
			});
		}
	}
}
