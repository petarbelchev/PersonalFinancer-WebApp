namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	using Data.Models;
	using static Data.DataConstants.Category;

	internal class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.HasData(new Category[]
			{
				new Category
				{
					Id = Guid.NewGuid(),
					Name = CategoryInitialBalanceName
				},
				new Category
				{
					Id = Guid.NewGuid(),
					Name = "Food & Drink"
				},
				new Category
				{
					Id = Guid.NewGuid(),
					Name = "Utilities"
				},
				new Category
				{
					Id = Guid.NewGuid(),
					Name = "Transport"
				},
				new Category
				{
					Id = Guid.NewGuid(),
					Name = "Housing"
				},
				new Category
				{
					Id = Guid.NewGuid(),
					Name = "Medical & Healthcare"
				}
			});
		}
	}
}
