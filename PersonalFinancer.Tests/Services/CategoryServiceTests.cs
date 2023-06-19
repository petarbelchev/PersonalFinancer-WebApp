namespace PersonalFinancer.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.ApiService;
    using PersonalFinancer.Services.ApiService.Models;

    internal class CategoryServiceTests : ServicesUnitTestsBase
    {
        private IEfRepository<Category> repo;
        private ApiService<Category> categoryService;

        [SetUp]
        public void SetUp()
        {
            this.repo = new EfRepository<Category>(this.sqlDbContext);
            this.categoryService = new ApiService<Category>(this.repo, this.mapper, this.memoryCache);
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewCategory_WithValidParams()
        {
            //Arrange
            string categoryName = "NewCategory";
            Guid ownerId = this.User1.Id;
            int countBefore = await this.repo.All().CountAsync();

            //Act
            ApiOutputServiceModel actual =
                await this.categoryService.CreateEntityAsync(categoryName, ownerId);

            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore + 1));
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(actual.Name, Is.EqualTo(categoryName));
            });
        }

        [Test]
        public async Task CreateEntity_ShouldRecreateDeletedBeforeCategory_WithValidParams()
        {
            //Arrange
            var deletedCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "DeletedCategory",
                OwnerId = this.User1.Id,
                IsDeleted = true
            };
            await this.repo.AddAsync(deletedCategory);
            await this.repo.SaveChangesAsync();
            int countBefore = await this.repo.All().CountAsync();

            string categoryName = deletedCategory.Name;
            Guid ownerId = this.User1.Id;

            //Assert
            Category? deletedAcc = await this.repo.FindAsync(deletedCategory.Id);
            Assert.That(deletedAcc, Is.Not.Null);
            Assert.That(deletedAcc!.IsDeleted, Is.True);

            //Act
            ApiOutputServiceModel result =
                await this.categoryService.CreateEntityAsync(categoryName, ownerId);

            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore));
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Id, Is.EqualTo(deletedCategory.Id));
                Assert.That(result.Name, Is.EqualTo(deletedCategory.Name));
            });
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewCategoryWhenAnotherUserHaveTheSameCategory()
        {
            //Arrange
            var user2Category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "User2Category",
                OwnerId = this.User2.Id
            };

            await this.repo.AddAsync(user2Category);
            await this.repo.SaveChangesAsync();
            int countBefore = await this.repo.All().CountAsync();

            string categoryName = user2Category.Name;
            Guid ownerId = this.User1.Id;

            //Assert
            Assert.That(await this.repo.FindAsync(user2Category.Id), Is.Not.Null);

            //Act
            ApiOutputServiceModel result =
                await this.categoryService.CreateEntityAsync(categoryName, ownerId);

            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore + 1));
                Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(result.Id, Is.Not.EqualTo(user2Category.Id));
                Assert.That(result.Name, Is.EqualTo(user2Category.Name));
            });
        }

        [Test]
        public void CreateEntity_ShouldThrowException_WhenCategoryExist()
        {
            //Arrange
            string categoryName = this.Cat2User1.Name;
            Guid ownerId = this.User1.Id;

            //Act & Assert
            Assert.That(async () => await this.categoryService.CreateEntityAsync(categoryName, ownerId),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Entity with the same name exist."));
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkCategoryAsDeleted_WithValidParams()
        {
            //Arrange
            var newCategory = new Category()
            {
                Id = Guid.NewGuid(),
                Name = "NewCategory",
                OwnerId = this.User1.Id
            };
            await this.repo.AddAsync(newCategory);
            await this.repo.SaveChangesAsync();

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(await this.repo.FindAsync(newCategory.Id), Is.Not.Null);
                Assert.That(newCategory.IsDeleted, Is.False);
            });

            //Act
            await this.categoryService.DeleteEntityAsync(newCategory.Id, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(newCategory.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(newCategory.Id), Is.Not.Null);
            });
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkCategoryAsDeleted_WhenUserIsAdmin()
        {
            //Arrange
            var newCategory = new Category()
            {
                Id = Guid.NewGuid(),
                Name = "NewCategory",
                OwnerId = this.User1.Id
            };
            await this.repo.AddAsync(newCategory);
            await this.repo.SaveChangesAsync();

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(await this.repo.FindAsync(newCategory.Id), Is.Not.Null);
                Assert.That(newCategory.IsDeleted, Is.False);
            });

            //Act
            await this.categoryService.DeleteEntityAsync(newCategory.Id, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(newCategory.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(newCategory.Id), Is.Not.Null);
            });
        }

        [Test]
        public void DeleteEntity_ShouldThrowException_WhenCategoryNotExist()
        {
            //Act & Assert
            Assert.That(async () => await this.categoryService
                  .DeleteEntityAsync(Guid.NewGuid(), this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task DeleteEntity_ShouldThrowException_WhenUserIsNotOwner()
        {
            //Arrange
            var user2Category = new Category()
            {
                Id = Guid.NewGuid(),
                Name = "ForDelete",
                OwnerId = this.User2.Id
            };
            await this.repo.AddAsync(user2Category);
            await this.repo.SaveChangesAsync();

            //Act & Assert
            Assert.That(async () => await this.categoryService
                  .DeleteEntityAsync(user2Category.Id, this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Unauthorized."));
        }
    }
}
