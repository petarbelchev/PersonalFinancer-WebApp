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
            this.repo = new EfRepository<Category>(this.dbContextMock);
			this.categoryService = new ApiService<Category>(this.repo, this.mapperMock);
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewCategory_WithValidParams()
        {
            //Arrange
            string categoryName = "NewCategory";
            int countBefore = await this.repo.All().CountAsync();

            //Act
            ApiEntityDTO actual = await this.categoryService
                .CreateEntityAsync(categoryName, this.User1.Id);

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
        public async Task CreateEntity_ShouldRecreateDeletedCategory_WithValidParams()
        {
            //Arrange
            Category category = this.Category4_User1_Deleted_WithoutTransactions;
			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO result = await this.categoryService
                .CreateEntityAsync(category.Name, this.User1.Id);

            //Arrange
            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore));

                this.AssertSamePropertiesValuesAreEqual(result, category);
            });
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewCategoryWhenAnotherUserHaveTheSameCategory()
        {
            //Arrange
            Category user2Category = this.Category5_User2_WithoutTransactions;
			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO result = await this.categoryService
                .CreateEntityAsync(user2Category.Name, this.User1.Id);

            //Arrange
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
            string categoryName = this.Category1_User1_WithTransactions.Name;
            Guid ownerId = this.User1.Id;

            //Act & Assert
            Assert.That(async () => await this.categoryService.CreateEntityAsync(categoryName, ownerId),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.ExistingEntityName));
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkCategoryAsDeleted_WithValidParams()
        {
            //Arrange
            Category category = this.Category2_User1_WithoutTransactions;

            //Act
            await this.categoryService.DeleteEntityAsync(category.Id, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(category.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(category.Id), Is.Not.Null);
            });
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkCategoryAsDeleted_WhenUserIsAdmin()
        {
            //Arrange
            Category category = this.Category2_User1_WithoutTransactions;

            //Act
            await this.categoryService.DeleteEntityAsync(category.Id, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(category.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(category.Id), Is.Not.Null);
            });
        }

        [Test]
        public void DeleteEntity_ShouldThrowException_WhenCategoryNotExist()
        {
            //Arrange
            var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.categoryService
                  .DeleteEntityAsync(invalidId, this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void DeleteEntity_ShouldThrowException_WhenUserIsNotOwner()
        {
            //Arrange
            Category category = this.Category2_User1_WithoutTransactions;

            //Act & Assert
            Assert.That(async () => await this.categoryService
                  .DeleteEntityAsync(category.Id, this.User2.Id, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
        }
    }
}
