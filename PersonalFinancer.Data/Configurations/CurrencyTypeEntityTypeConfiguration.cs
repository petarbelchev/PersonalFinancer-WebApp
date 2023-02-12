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
					Id = Guid.NewGuid(),
					Name = "BGN"
				},
				new Currency
				{
					Id = Guid.NewGuid(),
					Name = "EUR"
				},
				new Currency
				{
					Id = Guid.NewGuid(),
					Name = "USD"
				}
			});
		}
	}
}
