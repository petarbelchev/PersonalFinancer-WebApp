using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PersonalFinancer.Data.Models;

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
					Id = Guid.Parse("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"),
					Name = "BGN"
				},
				new Currency
				{
					Id = Guid.Parse("dab2761d-acb1-43bc-b56b-0d9c241c8882"),
					Name = "EUR"
				},
				new Currency
				{
					Id = Guid.Parse("2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f"),
					Name = "USD"
				}
			});
		}
	}
}
