using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.DataConstants.Category;

namespace PersonalFinancer.Data.Configurations
{
	internal class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.HasData(new Category[]
			{
				new Category
				{
					Id = 1,
					Name = CategoryInitialBalanceName
				},
				new Category
				{
					Id = 2,
					Name = "Food & Drink"
				},
				new Category
				{
					Id = 3,
					Name = "Utilities"
				},
				new Category
				{
					Id = 4,
					Name = "Transport"
				},
				new Category
				{
					Id = 5,
					Name = "Housing"
				},
				new Category
				{
					Id = 6,
					Name = "Medical & Healthcare"
				}
			});
		}
	}
}
