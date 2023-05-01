using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Models;
using PersonalFinancer.Data.Repositories;

using PersonalFinancer.Services.ApiService.Models;
using PersonalFinancer.Services.ApiService;
using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Tests.Services
{
	internal class CategoryServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<Category> repo;
		private ApiService<Category> categoryService;

		[SetUp]
		public void SetUp()
		{
			repo = new EfRepository<Category>(this.sqlDbContext);
			categoryService = new ApiService<Category>(repo, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task CreateEntity_ShouldAddNewCategory_WithValidParams()
		{
			//Arrange
			var inputModel = new CategoryInputModel
			{
				Name = "NewCategory",
				OwnerId = this.User1.Id
			};
			int countBefore = await repo.All().CountAsync();

			//Act
			ApiOutputServiceModel actual =
				await categoryService.CreateEntity(inputModel);

			int countAfter = await repo.All().CountAsync();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Id, Is.Not.Null);
			Assert.That(actual.Name, Is.EqualTo(inputModel.Name));
		}

		[Test]
		public async Task CreateEntity_ShouldRecreateDeletedBeforeCategory_WithValidParams()
		{
			//Arrange
			var deletedCategory = new Category
			{
				Id = Guid.NewGuid().ToString(),
				Name = "DeletedCategory",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			await repo.AddAsync(deletedCategory);
			await repo.SaveChangesAsync();
			int countBefore = await repo.All().CountAsync();

			var inputModel = new CategoryInputModel
			{
				Name = deletedCategory.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(async () =>
			{
				var deletedAcc = await repo.FindAsync(deletedCategory.Id);
				Assert.That(deletedAcc, Is.Not.Null);
				return deletedAcc.IsDeleted;
			},
			Is.True);

			//Act
			ApiOutputServiceModel result =
				await categoryService.CreateEntity(inputModel);

			int countAfter = await repo.All().CountAsync();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore));
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(deletedCategory.Id));
			Assert.That(result.Name, Is.EqualTo(deletedCategory.Name));
		}

		[Test]
		public async Task CreateEntity_ShouldAddNewCategoryWhenAnotherUserHaveTheSameCategory()
		{
			//Arrange
			var user2Category = new Category
			{
				Id = Guid.NewGuid().ToString(),
				Name = "User2Category",
				OwnerId = this.User2.Id
			};

			await repo.AddAsync(user2Category);
			await repo.SaveChangesAsync();
			int countBefore = await repo.All().CountAsync();

			var inputModel = new CategoryInputModel
			{
				Name = user2Category.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(await repo.FindAsync(user2Category.Id), Is.Not.Null);

			//Act
			ApiOutputServiceModel result =
				await categoryService.CreateEntity(inputModel);

			int countAfter = await repo.All().CountAsync();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(result.Id, Is.Not.Null);
			Assert.That(result.Id, Is.Not.EqualTo(user2Category.Id));
			Assert.That(result.Name, Is.EqualTo(user2Category.Name));
		}

		[Test]
		public void CreateEntity_ShouldThrowException_WhenCategoryExist()
		{
			//Arrange
			var inputModel = new CategoryInputModel
			{
				Name = this.Cat2User1.Name,
				OwnerId = this.User1.Id
			};

			//Act & Assert
			Assert.That(async () => await categoryService.CreateEntity(inputModel),
				Throws.TypeOf<ArgumentException>().With.Message
					.EqualTo("Entity with the same name exist."));
		}

		[Test]
		public async Task DeleteEntity_ShouldMarkCategoryAsDeleted_WithValidParams()
		{
			//Arrange
			var newCategory = new Category()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "NewCategory",
				OwnerId = this.User1.Id
			};
			await repo.AddAsync(newCategory);
			await repo.SaveChangesAsync();

			//Assert
			Assert.That(await repo.FindAsync(newCategory.Id), Is.Not.Null);
			Assert.That(newCategory.IsDeleted, Is.False);

			//Act
			await categoryService.DeleteEntity(newCategory.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(newCategory.IsDeleted, Is.True);
			Assert.That(await repo.FindAsync(newCategory.Id), Is.Not.Null);
		}
		
		[Test]
		public async Task DeleteEntity_ShouldMarkCategoryAsDeleted_WhenUserIsAdmin()
		{
			//Arrange
			var newCategory = new Category()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "NewCategory",
				OwnerId = this.User1.Id
			};
			await repo.AddAsync(newCategory);
			await repo.SaveChangesAsync();

			//Assert
			Assert.That(await repo.FindAsync(newCategory.Id), Is.Not.Null);
			Assert.That(newCategory.IsDeleted, Is.False);

			//Act
			await categoryService.DeleteEntity(newCategory.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			Assert.That(newCategory.IsDeleted, Is.True);
			Assert.That(await repo.FindAsync(newCategory.Id), Is.Not.Null);
		}

		[Test]
		public void DeleteEntity_ShouldThrowException_WhenCategoryNotExist()
		{
			//Act & Assert
			Assert.That(async () => await categoryService.DeleteEntity(
				Guid.NewGuid().ToString(), this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteEntity_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange
			var user2Category = new Category()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "ForDelete",
				OwnerId = this.User2.Id
			};
			await repo.AddAsync(user2Category);
			await repo.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await categoryService
				.DeleteEntity(user2Category.Id, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>()
					.With.Message.EqualTo("Unauthorized."));
		}
	}
}
