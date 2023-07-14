namespace PersonalFinancer.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Api;
    using PersonalFinancer.Services.Api.Models;

    internal class CurrencyServiceTests : ServicesUnitTestsBase
    {
        private IEfRepository<Currency> repo;
        private ApiService<Currency> currencyApiService;

        [SetUp]
        public void SetUp()
        {
            this.repo = new EfRepository<Currency>(this.dbContext);
            this.currencyApiService = new ApiService<Currency>(
                this.repo, this.mapper, this.memoryCache);
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewCurrency_WithValidParams()
        {
            //Arrange
            string currencyName = "NewCurrency";
            int countBefore = await this.repo.All().CountAsync();

            //Act
            ApiEntityDTO actual = await this.currencyApiService
                .CreateEntityAsync(currencyName, this.User1.Id);

            //Arrange
            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore + 1));
                Assert.That(actual.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(actual.Name, Is.EqualTo(currencyName));
            });
        }

        [Test]
        public async Task CreateEntity_ShouldRecreateDeletedCurrency_WithValidParams()
        {
            //Arrange
            Currency deletedCurrency = this.Currency4_User1_Deleted_WithoutAcc;
            int countBefore = await this.repo.All().CountAsync();

            //Act
            ApiEntityDTO result = await this.currencyApiService
                .CreateEntityAsync(deletedCurrency.Name, this.User1.Id);

            //Arrange
            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore));
                Assert.That(result, Is.Not.Null);

                AssertSamePropertiesValuesAreEqual(result, deletedCurrency);
            });
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewCurrencyWhenAnotherUserHaveTheSameCurrency()
        {
            //Arrange
            Currency user2Currency = this.Currency5_User2_WithoutAcc;
			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO result =
                await this.currencyApiService.CreateEntityAsync(user2Currency.Name, this.User1.Id);

            //Arrange
            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore + 1));
                Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(result.Id, Is.Not.EqualTo(user2Currency.Id));
                Assert.That(result.Name, Is.EqualTo(user2Currency.Name));
            });
        }

        [Test]
        public void CreateEntity_ShouldThrowException_WhenCurrencyExist()
        {
            //Arrange
            string currencyName = this.Currency1_User1_WithAcc.Name;

            //Act & Assert
            Assert.That(async () => await this.currencyApiService.CreateEntityAsync(currencyName, this.User1.Id),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.ExistingEntityName));
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkCurrencyAsDeleted_WithValidParams()
        {
            //Arrange
            Currency currency = this.Currency2_User1_WithoutAcc;

            //Act
            await this.currencyApiService.DeleteEntityAsync(currency.Id, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(currency.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(currency.Id), Is.Not.Null);
            });
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkCurrencyAsDeleted_WhenUserIsAdmin()
        {
			//Arrange
			Currency currency = this.Currency2_User1_WithoutAcc;

			//Act
			await this.currencyApiService.DeleteEntityAsync(currency.Id, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(currency.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(currency.Id), Is.Not.Null);
            });
        }

        [Test]
        public void DeleteEntity_ShouldThrowException_WhenCurrencyNotExist()
        {
            //Act & Assert
            Assert.That(async () => await this.currencyApiService
                  .DeleteEntityAsync(Guid.NewGuid(), this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void DeleteEntity_ShouldThrowException_WhenUserIsNotOwner()
        {
            //Arrange
            Currency currency = this.Currency1_User1_WithAcc;

            //Act & Assert
            Assert.That(async () => await this.currencyApiService
                  .DeleteEntityAsync(currency.Id, this.User2.Id, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
        }
    }
}
