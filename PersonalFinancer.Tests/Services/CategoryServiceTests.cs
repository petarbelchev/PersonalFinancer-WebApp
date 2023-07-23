namespace PersonalFinancer.Tests.Services
{
	using Microsoft.EntityFrameworkCore;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Api.Models;

	internal class CategoryServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<Category> repo;
		private ApiService<Category> categoryService;

		[SetUp]
		public void SetUp()
		{
			this.repo = new EfRepository<Category>(this.dbContext);
			this.categoryService = new ApiService<Category>(
				this.repo, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task CreateEntityAsync_ShouldAddNewCategory()
		{
			//Arrange
			string categoryName = "NewCategory";
			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO actual = await this.categoryService
				.CreateEntityAsync(categoryName, this.mainTestUserId);

			//Arrange
			int countAfter = await this.repo.All().CountAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(countAfter, Is.EqualTo(countBefore + 1));
				Assert.That(actual.Id, Is.Not.EqualTo(Guid.Empty));
				Assert.That(actual.Name, Is.EqualTo(categoryName));
			});
		}

		[Test]
		public async Task CreateEntityAsync_ShouldRecreateDeletedCategory()
		{
			//Arrange
			Category category = await this.repo.All()
				.Where(c => c.IsDeleted && c.OwnerId == this.mainTestUserId)
				.FirstAsync();

			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO result = await this.categoryService
				.CreateEntityAsync(category.Name, this.mainTestUserId);

			//Arrange
			int countAfter = await this.repo.All().CountAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(countAfter, Is.EqualTo(countBefore));

				AssertSamePropertiesValuesAreEqual(result, category);
			});
		}

		[Test]
		public async Task CreateEntityAsync_ShouldAddNewCategoryWhenAnotherUserHaveTheSameCategory()
		{
			//Arrange
			Category anotherUserCategory = await this.repo.All()
				.Where(c => c.OwnerId != this.mainTestUserId)
				.FirstAsync();

			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO result = await this.categoryService
				.CreateEntityAsync(anotherUserCategory.Name, this.mainTestUserId);

			//Arrange
			int countAfter = await this.repo.All().CountAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(countAfter, Is.EqualTo(countBefore + 1));
				Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
				Assert.That(result.Id, Is.Not.EqualTo(anotherUserCategory.Id));
				Assert.That(result.Name, Is.EqualTo(anotherUserCategory.Name));
			});
		}

		[Test]
		public async Task CreateEntityAsync_ShouldThrowArgumentException_WhenTheCategoryExistAndIsNotDeleted()
		{
			//Arrange
			string existingName = await this.repo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
				.Select(c => c.Name)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.categoryService.CreateEntityAsync(existingName, this.mainTestUserId),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.ExistingEntityName));
		}

		[Test]
		public async Task DeleteEntityAsync_ShouldMarkCategoryAsDeleted_WhenTheUserIsOwner()
		{
			//Arrange
			Category category = await this.repo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
				.FirstAsync();

			//Act
			await this.categoryService.DeleteEntityAsync(category.Id, this.mainTestUserId, isUserAdmin: false);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(category.IsDeleted, Is.True);
				Assert.That(await this.repo.FindAsync(category.Id), Is.Not.Null);
			});
		}

		[Test]
		public async Task DeleteEntityAsync_ShouldMarkCategoryAsDeleted_WhenTheUserIsAdmin()
		{
			//Arrange
			Category category = await this.repo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
				.FirstAsync();

			//Act
			await this.categoryService.DeleteEntityAsync(category.Id, this.adminId, isUserAdmin: true);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(category.IsDeleted, Is.True);
				Assert.That(await this.repo.FindAsync(category.Id), Is.Not.Null);
			});
		}

		[Test]
		public void DeleteEntityAsync_ShouldThrowInvalidOperationException_WhenTheCategoryDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.categoryService
				  .DeleteEntityAsync(invalidId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteEntityAsync_ShouldThrowArgumentException_WhenTheUserIsNotOwner()
		{
			//Arrange
			Category category = await this.repo.All()
				.Where(c => c.OwnerId != this.mainTestUserId && !c.IsDeleted)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.categoryService
				  .DeleteEntityAsync(category.Id, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}
	}
}
