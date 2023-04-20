using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.ApiService;
using PersonalFinancer.Services.ApiService.Models;

namespace PersonalFinancer.Tests.Services
{
	internal class CurrencyServiceTests : UnitTestsBase
	{
		private ApiService<Currency> currencyService;

		[SetUp]
		public void SetUp()
		{
			this.currencyService = new ApiService<Currency>(this.sqlDbContext, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task CreateEntity_ShouldAddNewCurrency_WithValidParams()
		{
			//Arrange
			var inputModel = new ApiInputServiceModel
			{
				Name = "NewCurrency",
				OwnerId = this.User1.Id
			};
			int countBefore = await sqlDbContext.Currencies.CountAsync();

			//Act
			ApiOutputServiceModel actual =
				await currencyService.CreateEntity(inputModel);

			int countAfter = await sqlDbContext.Currencies.CountAsync();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Id, Is.Not.Null);
			Assert.That(actual.Name, Is.EqualTo(inputModel.Name));
		}

		[Test]
		public async Task CreateEntity_ShouldRecreateDeletedBeforeCurrency_WithValidParams()
		{
			//Arrange
			var deletedCurrency = new Currency
			{
				Id = Guid.NewGuid().ToString(),
				Name = "DeletedCurrency",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			await sqlDbContext.Currencies.AddAsync(deletedCurrency);
			await sqlDbContext.SaveChangesAsync();
			int countBefore = await sqlDbContext.Currencies.CountAsync();

			var inputModel = new ApiInputServiceModel
			{
				Name = deletedCurrency.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(async () =>
			{
				var deletedAcc = await sqlDbContext.Currencies.FindAsync(deletedCurrency.Id);
				Assert.That(deletedAcc, Is.Not.Null);
				return deletedAcc.IsDeleted;
			},
			Is.True);

			//Act
			ApiOutputServiceModel result =
				await currencyService.CreateEntity(inputModel);

			int countAfter = await sqlDbContext.Currencies.CountAsync();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore));
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(deletedCurrency.Id));
			Assert.That(result.Name, Is.EqualTo(deletedCurrency.Name));
		}

		[Test]
		public async Task CreateEntity_ShouldAddNewCurrencyWhenAnotherUserHaveTheSameCurrency()
		{
			//Arrange
			var user2Currency = new Currency
			{
				Id = Guid.NewGuid().ToString(),
				Name = "User2Currency",
				OwnerId = this.User2.Id
			};

			await sqlDbContext.Currencies.AddAsync(user2Currency);
			await sqlDbContext.SaveChangesAsync();
			int countBefore = await sqlDbContext.Currencies.CountAsync();

			var inputModel = new ApiInputServiceModel
			{
				Name = user2Currency.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(await sqlDbContext.Currencies.FindAsync(user2Currency.Id), Is.Not.Null);

			//Act
			ApiOutputServiceModel result =
				await currencyService.CreateEntity(inputModel);

			int countAfter = await sqlDbContext.Currencies.CountAsync();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(result.Id, Is.Not.Null);
			Assert.That(result.Id, Is.Not.EqualTo(user2Currency.Id));
			Assert.That(result.Name, Is.EqualTo(user2Currency.Name));
		}

		[Test]
		public void CreateEntity_ShouldThrowException_WhenCurrencyExist()
		{
			//Arrange
			var inputModel = new ApiInputServiceModel
			{
				Name = this.Curr1User1.Name,
				OwnerId = this.User1.Id
			};

			//Act & Assert
			Assert.That(async () => await currencyService.CreateEntity(inputModel),
				Throws.TypeOf<ArgumentException>().With.Message
					.EqualTo("Entity with the same name exist."));
		}

		[Test]
		public async Task DeleteEntity_ShouldMarkCurrencyAsDeleted_WithValidParams()
		{
			//Arrange
			var newCategory = new Currency()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "NewCurrency",
				OwnerId = this.User1.Id
			};
			await sqlDbContext.Currencies.AddAsync(newCategory);
			await sqlDbContext.SaveChangesAsync();

			//Assert
			Assert.That(await sqlDbContext.Currencies.FindAsync(newCategory.Id), Is.Not.Null);
			Assert.That(newCategory.IsDeleted, Is.False);

			//Act
			await currencyService.DeleteEntity(newCategory.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(newCategory.IsDeleted, Is.True);
			Assert.That(await sqlDbContext.Currencies.FindAsync(newCategory.Id), Is.Not.Null);
		}
		
		[Test]
		public async Task DeleteEntity_ShouldMarkCurrencyAsDeleted_WhenUserIsAdmin()
		{
			//Arrange
			var newCurrency = new Currency()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "NewCurrency",
				OwnerId = this.User1.Id
			};
			await sqlDbContext.Currencies.AddAsync(newCurrency);
			await sqlDbContext.SaveChangesAsync();

			//Assert
			Assert.That(await sqlDbContext.Currencies.FindAsync(newCurrency.Id), Is.Not.Null);
			Assert.That(newCurrency.IsDeleted, Is.False);

			//Act
			await currencyService.DeleteEntity(newCurrency.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			Assert.That(newCurrency.IsDeleted, Is.True);
			Assert.That(await sqlDbContext.Currencies.FindAsync(newCurrency.Id), Is.Not.Null);
		}

		[Test]
		public void DeleteEntity_ShouldThrowException_WhenCurrencyNotExist()
		{
			//Act & Assert
			Assert.That(async () => await currencyService.DeleteEntity(
				Guid.NewGuid().ToString(), this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteEntity_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange
			var user2Currency = new Currency()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "ForDelete",
				OwnerId = this.User2.Id
			};
			await sqlDbContext.Currencies.AddAsync(user2Currency);
			await sqlDbContext.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await currencyService
				.DeleteEntity(user2Currency.Id, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>()
					.With.Message.EqualTo("Unauthorized."));
		}
	}
}
