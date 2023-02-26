namespace PersonalFinancer.Tests.Services
{
	using NUnit.Framework;

	using Data.Models;
	using static Data.DataConstants.Category;
	using PersonalFinancer.Services.Category;
	using PersonalFinancer.Services.Category.Models;

	[TestFixture]
	class CategoryServiceTests : UnitTestsBase
	{
		private ICategoryService categoryService;

		[SetUp]
		public void SetUp()
		{
			this.categoryService = new CategoryService(this.data, this.mapper);
		}

		[Test]
		public async Task DeleteCategory_ShouldDeleteCategory_WithValidData()
		{
			//Arrange
			Category category = new Category
			{
				Id = Guid.NewGuid(),
				Name = "TestCategory",
				UserId = this.User1.Id
			};
			data.Categories.Add(category);
			data.SaveChanges();

			//Assert that the Category is not deleted
			Assert.That(category.IsDeleted, Is.False);

			//Act
			await this.categoryService.DeleteCategory(category.Id, this.User1.Id);

			//Assert that the Category is deleted
			Assert.That(category.IsDeleted, Is.True);
		}

		[Test]
		public void DeleteCategory_ShouldThrowException_WithInvalidCategoryId()
		{
			//Act & Assert
			Assert.That(async () =>	await categoryService.DeleteCategory(Guid.NewGuid(), this.User1.Id),
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName")
					.EqualTo("Category does not exist."));
		}

		[Test]
		public void DeleteCategory_ShouldThrowException_WithInvalidUserId()
		{
			//Arrange
			Category category = new Category
			{
				Id = Guid.NewGuid(),
				Name = "TestCategory",
				UserId = this.User1.Id
			};
			data.Categories.Add(category);
			data.SaveChanges();

			//Act & Assert
			Assert.That(async () =>	await categoryService.DeleteCategory(category.Id, this.User2.Id),
				Throws.TypeOf<InvalidOperationException>().With.Message
					.EqualTo("You can't delete someone else category."));
		}

		[Test]
		public async Task CategoryById_ShouldReturnCorrectData_WithValidId()
		{
			//Arrange & Act
			CategoryViewModel? category = await categoryService
				.CategoryById(this.Category1.Id);

			//Assert
			Assert.That(category, Is.Not.Null);
			Assert.That(category.Id, Is.EqualTo(this.Category1.Id));
			Assert.That(category.Name, Is.EqualTo(this.Category1.Name));
		}

		[Test]
		public async Task CategoryById_ShouldReturnNull_WithInvalidId()
		{
			//Arrange & Act
			CategoryViewModel? category = await categoryService
				.CategoryById(Guid.NewGuid());

			//Assert
			Assert.That(category, Is.Null);
		}

		[Test]
		public async Task CreateCategory_ShouldAddNewCategory_WithValidData()
		{
			//Arrange
			string newCategoryName = "NewCategory";
			
			//Act
			CategoryViewModel newCategory = await categoryService
				.CreateCategory(this.User1.Id, newCategoryName);

			//Assert
			Assert.That(newCategory.Name, Is.EqualTo(newCategoryName));
			Assert.That(newCategory.UserId, Is.EqualTo(this.User1.Id));
		}

		[Test]
		public void CreateCategory_ShouldThrowException_WithExistingName()
		{
			//Act & Assert
			Assert.That(async () => await categoryService.CreateCategory(this.User1.Id, this.Category5.Name),
				Throws.TypeOf<InvalidOperationException>().With.Message
					.EqualTo("Category with the same name exist!"));
		}

		[Test]
		public async Task CreateCategory_ShouldRecreateDeletedCategory_WithValidData()
		{
			//Arrange: Add deleted Category to database
			Category category = new Category
			{
				Id = Guid.NewGuid(),
				Name = "DeletedCategory",
				UserId = this.User1.Id,
				IsDeleted = true
			};
			data.Categories.Add(category);
			data.SaveChanges();

			//Assert: The Category is deleted
			Assert.That(category.IsDeleted, Is.True);

			//Act: Recreate deleted Category
			CategoryViewModel recreatedCategory = await categoryService
				.CreateCategory(this.User1.Id, category.Name);

			//Assert: The Category is not deleted anymore and the data is correct
			Assert.That(category.IsDeleted, Is.False);
			Assert.That(recreatedCategory.Id, Is.EqualTo(category.Id));
			Assert.That(recreatedCategory.Name, Is.EqualTo(category.Name));
			Assert.That(recreatedCategory.UserId, Is.EqualTo(category.UserId));
		}

		[Test]
		public async Task IsInitialBalance_ShouldReturnTrue_WithInitialBalance()
		{
			//Act & Assert
			Assert.That(await categoryService.IsInitialBalance(this.Category1.Id), Is.True);
		}

		[Test]
		public async Task IsInitialBalance_ShouldReturnFalse_WithNotInitialBalance()
		{
			//Act & Assert
			Assert.That(await categoryService.IsInitialBalance(this.Category2.Id), Is.False);
		}

		[Test]
		public async Task IsInitialBalance_ShouldReturnFalse_WithNotExistingCategoryId()
		{
			//Act & Assert
			Assert.That(await categoryService.IsInitialBalance(Guid.NewGuid()), Is.False);
		}

		[Test]
		public async Task UserCategories_ShouldWorkCorrectly()
		{
			//Arrange: Get first user's categories where the user has custom category
			List<CategoryViewModel> expectedFirstUserCategories = data.Categories
				.Where(c => 
					c.Name != CategoryInitialBalanceName && !c.IsDeleted &&
					(c.UserId == null || c.UserId == this.User1.Id))
				.Select(c => mapper.Map<CategoryViewModel>(c))
				.ToList();
			
			//Arrange: Get second user's categories where the user hasn't custom categories
			List<CategoryViewModel> expectedSecondUserCategories = data.Categories
				.Where(c =>  
					c.Name != CategoryInitialBalanceName && !c.IsDeleted &&
					(c.UserId == null || c.UserId == this.User2.Id))
				.Select(c => mapper.Map<CategoryViewModel>(c))
				.ToList();

			//Act: Get actual users categories
			IEnumerable<CategoryViewModel> actualFirstUserCategories = await categoryService
				.UserCategories(this.User1.Id);

			IEnumerable<CategoryViewModel> actualSecondUserCategories = await categoryService
				.UserCategories(this.User2.Id);

			//Assert
			Assert.That(actualFirstUserCategories, 
				Is.Not.Null);

			Assert.That(actualFirstUserCategories.Count(), 
				Is.EqualTo(expectedFirstUserCategories.Count));

			for (int i = 0; i < actualFirstUserCategories.Count(); i++)
			{
				Assert.That(actualFirstUserCategories.ElementAt(i).Id, 
					Is.EqualTo(expectedFirstUserCategories.ElementAt(i).Id));
			}

			Assert.That(actualSecondUserCategories, 
				Is.Not.Null);

			Assert.That(actualSecondUserCategories.Count(), 
				Is.EqualTo(expectedSecondUserCategories.Count));

			for (int i = 0; i < actualSecondUserCategories.Count(); i++)
			{
				Assert.That(actualSecondUserCategories.ElementAt(i).Id, 
					Is.EqualTo(expectedSecondUserCategories.ElementAt(i).Id));
			}
		}
	}
}