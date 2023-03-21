using NUnit.Framework;

using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Categories.Models;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	class CategoryServiceTests : UnitTestsBase
	{
		private ICategoryService categoryService;

		[SetUp]
		public void SetUp()
		{
			this.categoryService = new CategoryService(this.data, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task DeleteCategory_ShouldDeleteCategory_WithValidData()
		{
			//Arrange
			Category category = new Category
			{
				Id = Guid.NewGuid().ToString(),
				Name = "TestCategory",
				OwnerId = this.User1.Id
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
			Assert.That(async () => await categoryService.DeleteCategory(Guid.NewGuid().ToString(), this.User1.Id),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void DeleteCategory_ShouldThrowException_WithInvalidUserId()
		{
			//Arrange
			Category category = new Category
			{
				Id = Guid.NewGuid().ToString(),
				Name = "TestCategory",
				OwnerId = this.User1.Id
			};
			data.Categories.Add(category);
			data.SaveChanges();

			//Act & Assert
			Assert.That(async () => await categoryService.DeleteCategory(category.Id, this.User2.Id),
				Throws.TypeOf<InvalidOperationException>().With.Message
					.EqualTo("You can't delete someone else category."));
		}

		[Test]
		public async Task CategoryById_ShouldReturnCorrectData_WithValidId()
		{
			//Arrange & Act
			CategoryViewModel? category = await categoryService
				.GetCategoryViewModel(this.Category1.Id, this.User1.Id);

			//Assert
			Assert.That(category, Is.Not.Null);
			Assert.That(category.Id, Is.EqualTo(this.Category1.Id));
			Assert.That(category.Name, Is.EqualTo(this.Category1.Name));
		}

		[Test]
		public void CategoryById_ShouldReturnNull_WithInvalidId()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await categoryService.GetCategoryViewModel(Guid.NewGuid().ToString(), this.User1.Id), 
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task CreateCategory_ShouldAddNewCategory_WithValidData()
		{
			//Arrange
			var newCategoryName = "NewCategory";

			//Act
			CategoryViewModel model = await categoryService.CreateCategory(this.User1.Id, newCategoryName);

			//Assert
			Assert.That(model.Id, Is.Not.Null);
			Assert.That(model.Name, Is.EqualTo(newCategoryName));
		}

		[Test]
		public void CreateCategory_ShouldThrowException_WithExistingName()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await categoryService.CreateCategory(this.User1.Id, this.Category2.Name),
				Throws.TypeOf<ArgumentException>().With.Message
					.EqualTo("Category with the same name exist!"));
		}

		[Test]
		[TestCase("A")]
		[TestCase("NameWith26CharactersLong!!")]
		public void CreateAccountType_ShouldThrowException_WithInvalidName(string categoryName)
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await categoryService.CreateCategory(this.User1.Id, categoryName),
				Throws.TypeOf<ArgumentException>().With.Message
					.EqualTo("Category name must be between 2 and 25 characters long."));
		}

		[Test]
		public async Task CreateCategory_ShouldRecreateDeletedCategory_WithValidData()
		{
			//Arrange: Add deleted Category to database
			Category category = new Category
			{
				Id = Guid.NewGuid().ToString(),
				Name = "DeletedCategory",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			data.Categories.Add(category);
			data.SaveChanges();

			//Assert: The Category is deleted
			Assert.That(category.IsDeleted, Is.True);

			//Act: Recreate deleted Category
			CategoryViewModel newCategory = await categoryService
				.CreateCategory(this.User1.Id, category.Name);

			//Assert: The Category is not deleted anymore and the data is correct
			Assert.That(category.IsDeleted, Is.False);
			Assert.That(newCategory.Id, Is.EqualTo(category.Id));
			Assert.That(newCategory.Name, Is.EqualTo(category.Name));
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
		public void IsInitialBalance_ShouldReturnFalse_WithNotExistingCategoryId()
		{
			//Act & Assert
			Assert.That(async () => await categoryService.IsInitialBalance(Guid.NewGuid().ToString()), 
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task UserCategories_ShouldWorkCorrectly()
		{
			//Arrange: Get first user categories where the user has custom category
			List<CategoryViewModel> expectedFirstUserCategories = data.Categories
				.Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
				.OrderBy(c => c.Name)
				.Select(c => mapper.Map<CategoryViewModel>(c))
				.ToList();

			//Arrange: Get second user categories where the user hasn't custom categories
			List<CategoryViewModel> expectedSecondUserCategories = data.Categories
				.Where(c => c.OwnerId == this.User2.Id && !c.IsDeleted)
				.OrderBy(c => c.Name)
				.Select(c => mapper.Map<CategoryViewModel>(c))
				.ToList();

			//Act: Get actual users categories
			IEnumerable<CategoryViewModel> actualFirstUserCategories = await categoryService
				.GetUserCategories(this.User1.Id);

			IEnumerable<CategoryViewModel> actualSecondUserCategories = await categoryService
				.GetUserCategories(this.User2.Id);

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

		[Test]
		public async Task CategoryIdByName_ShouldReturnId_WithValidCategoryName()
		{
			//Arrange & Act
			string categoryId = await categoryService.GetCategoryIdByName(this.Category1.Name);

			//Assert
			Assert.That(categoryId, Is.EqualTo(this.Category1.Id));
		}

		[Test]
		public void CategoryIdByName_ShouldThrowException_WithInvalidCategoryName()
		{
			//Act & Assert
			Assert.That(async () => await categoryService.GetCategoryIdByName("NotExistCategory"),
				Throws.TypeOf<InvalidOperationException>());
		}
	}
}