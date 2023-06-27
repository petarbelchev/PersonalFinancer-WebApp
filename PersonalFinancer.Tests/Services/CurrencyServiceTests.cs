namespace PersonalFinancer.Tests.Services
{
	using Microsoft.EntityFrameworkCore;
	using NUnit.Framework;
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
            this.repo = new EfRepository<Currency>(this.sqlDbContext);
            this.currencyApiService = new ApiService<Currency>(this.repo, this.mapper);
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewCurrency_WithValidParams()
        {
            //Arrange
            string currencyName = "NewCurrency";
            Guid ownerId = this.User1.Id;
            int countBefore = await this.repo.All().CountAsync();

            //Act
            ApiEntityDTO actual =
                await this.currencyApiService.CreateEntityAsync(currencyName, ownerId);

            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore + 1));
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(actual.Name, Is.EqualTo(currencyName));
            });
        }

        [Test]
        public async Task CreateEntity_ShouldRecreateDeletedBeforeCurrency_WithValidParams()
        {
            //Arrange
            var deletedCurrency = new Currency
            {
                Id = Guid.NewGuid(),
                Name = "DeletedCurrency",
                OwnerId = this.User1.Id,
                IsDeleted = true
            };
            await this.repo.AddAsync(deletedCurrency);
            await this.repo.SaveChangesAsync();
            int countBefore = await this.repo.All().CountAsync();

            string currencyName = deletedCurrency.Name;
            Guid ownerId = this.User1.Id;

            //Assert
            Currency? deletedAcc = await this.repo.FindAsync(deletedCurrency.Id);
            Assert.That(deletedAcc, Is.Not.Null);
            Assert.That(deletedAcc.IsDeleted, Is.True);

            //Act
            ApiEntityDTO result =
                await this.currencyApiService.CreateEntityAsync(currencyName, ownerId);

            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore));
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Id, Is.EqualTo(deletedCurrency.Id));
                Assert.That(result.Name, Is.EqualTo(deletedCurrency.Name));
            });
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewCurrencyWhenAnotherUserHaveTheSameCurrency()
        {
            //Arrange
            var user2Currency = new Currency
            {
                Id = Guid.NewGuid(),
                Name = "User2Currency",
                OwnerId = this.User2.Id
            };

            await this.repo.AddAsync(user2Currency);
            await this.repo.SaveChangesAsync();
            int countBefore = await this.repo.All().CountAsync();

            string currencyName = user2Currency.Name;
            Guid ownerId = this.User1.Id;

            //Assert
            Assert.That(await this.repo.FindAsync(user2Currency.Id), Is.Not.Null);

            //Act
            ApiEntityDTO result =
                await this.currencyApiService.CreateEntityAsync(currencyName, ownerId);

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
            Guid ownerId = this.User1.Id;

            //Act & Assert
            Assert.That(async () => await this.currencyApiService.CreateEntityAsync(currencyName, ownerId),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Entity with the same name exist."));
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkCurrencyAsDeleted_WithValidParams()
        {
            //Arrange
            var newCategory = new Currency()
            {
                Id = Guid.NewGuid(),
                Name = "NewCurrency",
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
            await this.currencyApiService.DeleteEntityAsync(newCategory.Id, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(newCategory.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(newCategory.Id), Is.Not.Null);
            });
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkCurrencyAsDeleted_WhenUserIsAdmin()
        {
            //Arrange
            var newCurrency = new Currency()
            {
                Id = Guid.NewGuid(),
                Name = "NewCurrency",
                OwnerId = this.User1.Id
            };
            await this.repo.AddAsync(newCurrency);
            await this.repo.SaveChangesAsync();

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(await this.repo.FindAsync(newCurrency.Id), Is.Not.Null);
                Assert.That(newCurrency.IsDeleted, Is.False);
            });

            //Act
            await this.currencyApiService.DeleteEntityAsync(newCurrency.Id, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(newCurrency.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(newCurrency.Id), Is.Not.Null);
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
        public async Task DeleteEntity_ShouldThrowException_WhenUserIsNotOwner()
        {
            //Arrange
            var user2Currency = new Currency()
            {
                Id = Guid.NewGuid(),
                Name = "ForDelete",
                OwnerId = this.User2.Id
            };
            await this.repo.AddAsync(user2Currency);
            await this.repo.SaveChangesAsync();

            //Act & Assert
            Assert.That(async () => await this.currencyApiService
                  .DeleteEntityAsync(user2Currency.Id, this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Unauthorized."));
        }
    }
}
