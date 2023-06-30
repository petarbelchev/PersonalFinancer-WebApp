namespace PersonalFinancer.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Api;
    using PersonalFinancer.Services.Api.Models;

    internal class AccountTypeServiceTests : ServicesUnitTestsBase
    {
        private IEfRepository<AccountType> repo;
		private ApiService<AccountType> accountTypeService;

        [SetUp]
        public void SetUp()
        {
            this.repo = new EfRepository<AccountType>(this.dbContextMock);
			this.accountTypeService = new ApiService<AccountType>(this.repo, this.mapperMock);
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewAccountType_WithValidParams()
        {
            //Arrange
            string accountTypeName = "NewAccountType";
            Guid ownerId = this.User1.Id;
            int countBefore = await this.repo.All().CountAsync();

            //Act
            ApiEntityDTO actual =
                await this.accountTypeService.CreateEntityAsync(accountTypeName, ownerId);

            //Arrange
            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore + 1));
                Assert.That(actual.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(actual.Name, Is.EqualTo(accountTypeName));
            });
        }

        [Test]
        public async Task CreateEntity_ShouldRecreateDeletedAccountType_WithValidParams()
        {
            //Arrange
            AccountType deletedAccType = this.AccType4_User1_Deleted_WithoutAcc;
            int countBefore = await this.repo.All().CountAsync();
            string accountTypeName = deletedAccType.Name;

            //Act
            ApiEntityDTO result = await this.accountTypeService
                .CreateEntityAsync(accountTypeName, this.User1.Id);

            //Arrange
            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore));

                this.AssertSamePropertiesValuesAreEqual(result, deletedAccType);
            });
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewAccTypeWhenAnotherUserHaveTheSameAccType()
        {
            //Arrange
            AccountType user2AccountType = this.AccType5_User2_WithoutAcc;
            int countBefore = await this.repo.All().CountAsync();
            string accountTypeName = user2AccountType.Name;

            //Act
            ApiEntityDTO result = await this.accountTypeService
                .CreateEntityAsync(accountTypeName, this.User1.Id);

            //Arrange
            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore + 1));
                Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(result.Id, Is.Not.EqualTo(user2AccountType.Id));
                Assert.That(result.Name, Is.EqualTo(user2AccountType.Name));
            });
        }

        [Test]
        public void CreateEntity_ShouldThrowException_WhenAccTypeExist()
        {
            //Arrange
            string accountTypeName = this.AccType1_User1_WithAcc.Name;
            Guid ownerId = this.User1.Id;

            //Act & Assert
            Assert.That(async () => await this.accountTypeService.CreateEntityAsync(accountTypeName, ownerId),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.ExistingEntityName));
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkAccTypeAsDeleted_WithValidParams()
        {
            //Arrange
            AccountType accountType = this.AccType1_User1_WithAcc;

            //Act
            await this.accountTypeService.DeleteEntityAsync(accountType.Id, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(accountType.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(accountType.Id), Is.Not.Null);
            });
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkAccTypeAsDeleted_WhenUserIsAdmin()
        {
			//Arrange
			AccountType accountType = this.AccType1_User1_WithAcc;

			//Act
			await this.accountTypeService.DeleteEntityAsync(accountType.Id, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(accountType.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(accountType.Id), Is.Not.Null);
            });
        }

        [Test]
        public void DeleteEntity_ShouldThrowException_WhenAccTypeNotExist()
        {
            //Arrange
            var invalidId = Guid.NewGuid();

            //Act & Assert
            Assert.That(async () => await this.accountTypeService
                  .DeleteEntityAsync(invalidId, this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void DeleteEntity_ShouldThrowException_WhenUserIsNotOwner()
        {
			//Arrange
			AccountType accountType = this.AccType1_User1_WithAcc;

			//Act & Assert
			Assert.That(async () => await this.accountTypeService
                  .DeleteEntityAsync(accountType.Id, this.User2.Id, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
        }
    }
}
