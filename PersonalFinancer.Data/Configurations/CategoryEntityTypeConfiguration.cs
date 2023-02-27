namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	using Data.Models;
	using static Data.DataConstants.CategoryConstants;

	internal class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.HasData(new Category[]
			{
				new Category
				{
					Id = Guid.Parse("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
					Name = CategoryInitialBalanceName
				},
				new Category
				{
					Id = Guid.Parse("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"),
					Name = "Food & Drink"
				},
				new Category
				{
					Id = Guid.Parse("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"),
					Name = "Utilities"
				},
				new Category
				{
					Id = Guid.Parse("b58a7947-eecf-40d0-b84e-c6947fcbfd86"),
					Name = "Transport"
				},
				new Category
				{
					Id = Guid.Parse("96e441e3-c5a6-427f-bb32-85940242d9ee"),
					Name = "Medical & Healthcare"
				},
				new Category
				{
					Id = Guid.Parse("081a7be8-15c4-426e-872c-dfaf805e3fec"),
					Name = "Salary"
				}
			});
		}
	}
}
