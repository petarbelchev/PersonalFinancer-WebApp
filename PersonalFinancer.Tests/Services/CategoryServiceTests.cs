﻿namespace PersonalFinancer.Tests.Services
{
	using Microsoft.EntityFrameworkCore;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Constants;
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
				this.repo, this.mapper, this.cacheMock.Object);
		}

		[Test]
		public async Task CreateEntityAsync_ShouldAddNewCategory()
		{
			//Arrange
			string categoryName = "NewCategory";
			int countBefore = await this.repo.All().CountAsync();

			string cacheKey = CacheConstants.AccountsAndCategoriesKey + this.mainTestUserId;

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

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
		}

		[Test]
		public async Task CreateEntityAsync_ShouldRecreateDeletedCategory()
		{
			//Arrange
			Category category = await this.repo.All()
				.Where(c => c.IsDeleted && c.OwnerId == this.mainTestUserId)
				.FirstAsync();

			string cacheKey = CacheConstants.AccountsAndCategoriesKey + this.mainTestUserId;

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

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
		}

		[Test]
		public async Task CreateEntityAsync_ShouldAddNewCategoryWhenAnotherUserHaveTheSameCategory()
		{
			//Arrange
			Category anotherUserCategory = await this.repo.All()
				.Where(c => c.OwnerId != this.mainTestUserId)
				.FirstAsync();

			string cacheKey = CacheConstants.AccountsAndCategoriesKey + this.mainTestUserId;

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

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
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
		[TestCase(false)]
		[TestCase(true)]
		public async Task DeleteEntityAsync_ShouldMarkCategoryAsDeleted_WhenTheUserIsOwner(bool isUserAdmin)
		{
			//Arrange
			Category category = await this.repo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
				.FirstAsync();

			string cacheKey = CacheConstants.AccountsAndCategoriesKey + this.mainTestUserId;

			Guid currentUserId = isUserAdmin ? this.adminId : this.mainTestUserId;

			//Act
			await this.categoryService.DeleteEntityAsync(category.Id, currentUserId, isUserAdmin);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(category.IsDeleted, Is.True);
				Assert.That(await this.repo.FindAsync(category.Id), Is.Not.Null);
			});

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
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
		public async Task DeleteEntityAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			Category category = await this.repo.All()
				.Where(c => c.OwnerId != this.mainTestUserId && !c.IsDeleted)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.categoryService
				  .DeleteEntityAsync(category.Id, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}
	}
}
