namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	using Data.Models;

	internal class CurrencyTypeEntityTypeConfiguration : IEntityTypeConfiguration<Currency>
	{
		public void Configure(EntityTypeBuilder<Currency> builder)
		{
			builder.HasData(new Currency[]
			{
				new Currency
				{
					Id = 1,
					Name = "BGN"
				},
				new Currency
				{
					Id = 2,
					Name = "EUR"
				},
				new Currency
				{
					Id = 3,
					Name = "USD"
				}
			});
		}
	}
}
