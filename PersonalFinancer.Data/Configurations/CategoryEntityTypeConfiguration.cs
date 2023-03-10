using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.Constants.CategoryConstants;
using static PersonalFinancer.Data.Constants.SeedConstants;

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
					Id = Guid.Parse(InitialBalanceCategoryId),
					Name = CategoryInitialBalanceName
				},
				new Category
				{
					Id = Guid.Parse(FoodDrinkCategoryId),
					Name = "Food & Drink",
					UserId = FirstUserId
				},
				new Category
				{
					Id = Guid.Parse(UtilitiesCategoryId),
					Name = "Utilities",
					UserId = FirstUserId
				},
				new Category
				{
					Id = Guid.Parse(TransportCategoryId),
					Name = "Transport",
					UserId = FirstUserId
				},
				new Category
				{
					Id = Guid.Parse(MedicalHealthcareCategoryId),
					Name = "Medical & Healthcare",
					UserId = FirstUserId
				},
				new Category
				{
					Id = Guid.Parse(SalaryCategoryId),
					Name = "Salary",
					UserId = FirstUserId
				},
				new Category
				{
					Id = Guid.Parse(MoneyTransferCategoryId),
					Name = "Money Transfer",
					UserId = FirstUserId
				},
				new Category
				{
					Id = Guid.Parse(DividentsCategoryId),
					Name = "Dividents",
					UserId = FirstUserId
				}
			});
		}
	}
}
