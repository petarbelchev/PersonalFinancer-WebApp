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
					Id = InitialBalanceCategoryId,
					Name = CategoryInitialBalanceName,
					UserId = AdminId
				},
				new Category
				{
					Id = FoodDrinkCategoryId,
					Name = "Food & Drink",
					UserId = FirstUserId
				},
				new Category
				{
					Id = UtilitiesCategoryId,
					Name = "Utilities",
					UserId = FirstUserId
				},
				new Category
				{
					Id = TransportCategoryId,
					Name = "Transport",
					UserId = FirstUserId
				},
				new Category
				{
					Id = MedicalHealthcareCategoryId,
					Name = "Medical & Healthcare",
					UserId = FirstUserId
				},
				new Category
				{
					Id = SalaryCategoryId,
					Name = "Salary",
					UserId = FirstUserId
				},
				new Category
				{
					Id = MoneyTransferCategoryId,
					Name = "Money Transfer",
					UserId = FirstUserId
				},
				new Category
				{
					Id = DividentsCategoryId,
					Name = "Dividents",
					UserId = FirstUserId
				}
			});
		}
	}
}
