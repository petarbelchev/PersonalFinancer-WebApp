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
					Id = BgnCurrencyId,
					Name = "BGN",
					UserId = FirstUserId
				},
				new Currency
				{
					Id = EurCurrencyId,
					Name = "EUR",
					UserId = FirstUserId
				},
				new Currency
				{
					Id = UsdCurrencyId,
					Name = "USD",
					UserId = FirstUserId
				}
			});
		}
	}
}
