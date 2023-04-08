namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	using Models;
	using static Constants;

	internal class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.HasData(new Category[]
			{
				new Category
				{
					Id = CategoryConstants.InitialBalanceCategoryId,
					Name = CategoryConstants.CategoryInitialBalanceName,
					OwnerId = SeedConstants.AdminId
				},
				new Category
				{
					Id = SeedConstants.FoodDrinkCategoryId,
					Name = "Food & Drink",
					OwnerId = SeedConstants.FirstUserId
				},
				new Category
				{
					Id = SeedConstants.UtilitiesCategoryId,
					Name = "Utilities",
					OwnerId = SeedConstants.FirstUserId
				},
				new Category
				{
					Id = SeedConstants.TransportCategoryId,
					Name = "Transport",
					OwnerId = SeedConstants.FirstUserId
				},
				new Category
				{
					Id = SeedConstants.MedicalHealthcareCategoryId,
					Name = "Medical & Healthcare",
					OwnerId = SeedConstants.FirstUserId
				},
				new Category
				{
					Id = SeedConstants.SalaryCategoryId,
					Name = "Salary",
					OwnerId = SeedConstants.FirstUserId
				},
				new Category
				{
					Id = SeedConstants.MoneyTransferCategoryId,
					Name = "Money Transfer",
					OwnerId = SeedConstants.FirstUserId
				},
				new Category
				{
					Id = SeedConstants.DividentsCategoryId,
					Name = "Dividents",
					OwnerId = SeedConstants.FirstUserId
				}
			});
		}
	}
}
