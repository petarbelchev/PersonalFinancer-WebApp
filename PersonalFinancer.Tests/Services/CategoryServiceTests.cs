//using NUnit.Framework;

//using PersonalFinancer.Data.Models;
//using PersonalFinancer.Services.Categories;
//using PersonalFinancer.Services.Categories.Models;
//using PersonalFinancer.Services.Shared.Models;

//namespace PersonalFinancer.Tests.Services
//{
//	internal class CategoryServiceTests : UnitTestsBase
//	{
//		private ICategoryService categoryService;

//		[SetUp]
//		public void SetUp()
//		{
//			this.categoryService = new CategoryService(this.data, memoryCache, this.mapper);
//		}
		
//		[Test]
//		public async Task CreateCategory_ShouldAddNewCategory_WithValidData()
//		{
//			//Arrange
//			var inputModel = new CategoryInputModel
//			{
//				Name = "NewCategory",
//				OwnerId = this.User1.Id
//			};

//			//Act
//			CategoryViewModel viewModel = await categoryService.CreateCategory(inputModel);

//			//Assert
//			Assert.That(viewModel.Id, Is.Not.Null);
//			Assert.That(viewModel.Name, Is.EqualTo(inputModel.Name));
//		}

//		[Test]
//		public void CreateCategory_ShouldThrowException_WithExistingName()
//		{
//			//Arrange
//			var inputModel = new CategoryInputModel
//			{
//				Name = this.Category2.Name,
//				OwnerId = this.User1.Id
//			};

//			//Act & Assert
//			Assert.That(async () => await categoryService.CreateCategory(inputModel),
//				Throws.TypeOf<ArgumentException>().With.Message
//					.EqualTo("Category with the same name exist!"));
//		}

//		[Test]
//		public async Task CreateCategory_ShouldRecreateDeletedCategory_WithValidData()
//		{
//			//Arrange: Add deleted Category to database
//			Category category = new Category
//			{
//				Id = Guid.NewGuid().ToString(),
//				Name = "DeletedCategory",
//				OwnerId = this.User1.Id,
//				IsDeleted = true
//			};
//			data.Categories.Add(category);
//			data.SaveChanges();

//			var inputModel = new CategoryInputModel
//			{
//				Name = category.Name,
//				OwnerId = this.User1.Id
//			};

//			//Assert: The Category is deleted
//			Assert.That(category.IsDeleted, Is.True);

//			//Act: Recreate deleted Category
//			CategoryViewModel newCategory = 
//				await categoryService.CreateCategory(inputModel);

//			//Assert: The Category is not deleted anymore and the data is correct
//			Assert.That(category.IsDeleted, Is.False);
//			Assert.That(newCategory.Id, Is.EqualTo(category.Id));
//			Assert.That(newCategory.Name, Is.EqualTo(category.Name));
//		}

//		[Test]
//		public async Task DeleteCategory_ShouldDeleteCategory_WithValidData()
//		{
//			//Arrange
//			Category category = new Category
//			{
//				Id = Guid.NewGuid().ToString(),
//				Name = "TestCategory",
//				OwnerId = this.User1.Id
//			};
//			data.Categories.Add(category);
//			data.SaveChanges();

//			//Assert that the Category is not deleted
//			Assert.That(category.IsDeleted, Is.False);

//			//Act
//			await this.categoryService.DeleteCategory(category.Id, this.User1.Id);

//			//Assert that the Category is deleted
//			Assert.That(category.IsDeleted, Is.True);
//		}

//		[Test]
//		public void DeleteCategory_ShouldThrowException_WithInvalidCategoryId()
//		{
//			//Act & Assert
//			Assert.That(async () => await categoryService.DeleteCategory(Guid.NewGuid().ToString(), this.User1.Id),
//				Throws.TypeOf<InvalidOperationException>());
//		}

//		[Test]
//		public async Task DeleteCategory_ShouldThrowException_WithInvalidUserId()
//		{
//			//Arrange
//			Category category = new Category
//			{
//				Id = Guid.NewGuid().ToString(),
//				Name = "TestCategory",
//				OwnerId = this.User1.Id
//			};
//			await data.Categories.AddAsync(category);
//			await data.SaveChangesAsync();

//			//Act & Assert
//			Assert.That(async () => await categoryService.DeleteCategory(category.Id, this.User2.Id),
//				Throws.TypeOf<ArgumentException>().With.Message
//					.EqualTo("Can't delete someone else category."));
//		}
//	}
//}
