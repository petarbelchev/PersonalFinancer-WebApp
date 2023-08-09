namespace PersonalFinancer.Tests.Services
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

	internal class CurrencyServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<Currency> repo;
		private ApiService<Currency> currencyApiService;

		[SetUp]
		public void SetUp()
		{
			this.repo = new EfRepository<Currency>(this.dbContext);
			this.currencyApiService = new ApiService<Currency>(
				this.repo, this.mapper, this.cacheMock.Object);
		}

		[Test]
		public async Task CreateEntityAsync_ShouldAddNewCurrency()
		{
			//Arrange
			string currencyName = "NewCurrency";
			int countBefore = await this.repo.All().CountAsync();

			string cacheKey = CacheConstants.AccountTypesAndCurrenciesKey + this.mainTestUserId;

			//Act
			ApiEntityDTO actual = await this.currencyApiService
				.CreateEntityAsync(currencyName, this.mainTestUserId);

			//Arrange
			int countAfter = await this.repo.All().CountAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(countAfter, Is.EqualTo(countBefore + 1));
				Assert.That(actual.Id, Is.Not.EqualTo(Guid.Empty));
				Assert.That(actual.Name, Is.EqualTo(currencyName));
			});

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
		}

		[Test]
		public async Task CreateEntityAsync_ShouldRecreateDeletedCurrency()
		{
			//Arrange
			Currency deletedCurrency = await this.repo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && c.IsDeleted)
				.FirstAsync();

			string cacheKey = CacheConstants.AccountTypesAndCurrenciesKey + this.mainTestUserId;

			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO result = await this.currencyApiService
				.CreateEntityAsync(deletedCurrency.Name, this.mainTestUserId);

			//Arrange
			int countAfter = await this.repo.All().CountAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(countAfter, Is.EqualTo(countBefore));
				Assert.That(result, Is.Not.Null);

				AssertSamePropertiesValuesAreEqual(result, deletedCurrency);
			});

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
		}

		[Test]
		public async Task CreateEntityAsync_ShouldAddNewCurrencyWhenAnotherUserHaveTheSameCurrency()
		{
			//Arrange
			Currency anotherUserCurrency = await this.repo.All()
				.Where(c => c.OwnerId != this.mainTestUserId)
				.FirstAsync();

			string cacheKey = CacheConstants.AccountTypesAndCurrenciesKey + this.mainTestUserId;

			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO result = await this.currencyApiService
				.CreateEntityAsync(anotherUserCurrency.Name, this.mainTestUserId);

			//Arrange
			int countAfter = await this.repo.All().CountAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(countAfter, Is.EqualTo(countBefore + 1));
				Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
				Assert.That(result.Id, Is.Not.EqualTo(anotherUserCurrency.Id));
				Assert.That(result.Name, Is.EqualTo(anotherUserCurrency.Name));
			});

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
		}

		[Test]
		public async Task CreateEntityAsync_ShouldThrowArgumentException_WhenTheCurrencyExistAndIsNotDeleted()
		{
			//Arrange
			string existingName = await this.repo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
				.Select(c => c.Name)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.currencyApiService.CreateEntityAsync(existingName, this.mainTestUserId),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.ExistingEntityName));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task DeleteEntityAsync_ShouldMarkCurrencyAsDeleted(bool isUserAdmin)
		{
			//Arrange
			Currency currency = await this.repo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
				.FirstAsync();

			string cacheKey = CacheConstants.AccountTypesAndCurrenciesKey + this.mainTestUserId;

			Guid currentUserId = isUserAdmin ? this.adminId : this.mainTestUserId;

			//Act
			await this.currencyApiService.DeleteEntityAsync(currency.Id, currentUserId, isUserAdmin);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(currency.IsDeleted, Is.True);
				Assert.That(await this.repo.FindAsync(currency.Id), Is.Not.Null);
			});

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
		}

		[Test]
		public void DeleteEntityAsync_ShouldThrowInvalidOperationException_WhenTheCurrencyDoesNotExist()
		{
			//Act & Assert
			Assert.That(async () => await this.currencyApiService
				  .DeleteEntityAsync(Guid.NewGuid(), this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteEntityAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			Currency currency = await this.repo.All()
				.Where(c => c.OwnerId != this.mainTestUserId && !c.IsDeleted)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.currencyApiService
				  .DeleteEntityAsync(currency.Id, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}
	}
}
