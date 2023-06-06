using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.Constants.SeedConstants;

namespace PersonalFinancer.Data.Configurations
{
	internal class CurrencyTypeEntityTypeConfiguration : IEntityTypeConfiguration<Currency>
	{
		public void Configure(EntityTypeBuilder<Currency> builder)
		{
			builder.HasData(new Currency[]
			{
				new Currency
				{
					Id = FirstUserBGNCurrencyId,
					Name = "BGN",
					OwnerId = FirstUserId
				},
				new Currency
				{
					Id = FirstUserEURCurrencyId,
					Name = "EUR",
					OwnerId = FirstUserId
				},
				new Currency
				{
					Id = FirstUserUSDCurrencyId,
					Name = "USD",
					OwnerId = FirstUserId
				}
			});
		}
	}
}
