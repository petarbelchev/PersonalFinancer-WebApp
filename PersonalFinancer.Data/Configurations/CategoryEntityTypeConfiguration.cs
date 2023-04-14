namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	using Data.Models;
	using static Data.Constants.CategoryConstants;
	using static Data.Constants.SeedConstants;

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
					OwnerId = AdminId
				},
				new Category
				{
					Id = FoodDrinkCategoryId,
					Name = "Food & Drink",
					OwnerId = FirstUserId
				},
				new Category
				{
					Id = UtilitiesCategoryId,
					Name = "Utilities",
					OwnerId = FirstUserId
				},
				new Category
				{
					Id = TransportCategoryId,
					Name = "Transport",
					OwnerId = FirstUserId
				},
				new Category
				{
					Id = MedicalHealthcareCategoryId,
					Name = "Medical & Healthcare",
					OwnerId = FirstUserId
				},
				new Category
				{
					Id = SalaryCategoryId,
					Name = "Salary",
					OwnerId = FirstUserId
				},
				new Category
				{
					Id = MoneyTransferCategoryId,
					Name = "Money Transfer",
					OwnerId = FirstUserId
				},
				new Category
				{
					Id = DividentsCategoryId,
					Name = "Dividents",
					OwnerId = FirstUserId
				}
			});
		}
	}
}
