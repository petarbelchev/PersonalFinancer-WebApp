//using Microsoft.EntityFrameworkCore;
//using NUnit.Framework;

//using PersonalFinancer.Data.Models;
//using PersonalFinancer.Services.Currencies;
//using PersonalFinancer.Services.Currencies.Models;

//namespace PersonalFinancer.Tests.Services
//{
//	[TestFixture]
//	class CurrencyServiceTests : UnitTestsBase
//	{
//		private ICurrencyService currencyService;

//		[SetUp]
//		public void SetUp()
//		{
//			this.currencyService = new CurrencyService(this.data, this.mapper, this.memoryCache);
//		}

//		[Test]
//		public async Task CreateCurrency_ShouldAddNewCurrency_WithValidParams()
//		{
//			//Arrange
//			int currenciesBefore = data.Currencies
//				.Count(c => c.OwnerId == this.User1.Id && !c.IsDeleted);

//			var model = new CurrencyViewModel { Name = "NEW" };

//			//Assert
//			Assert.That(await data.Currencies.FirstOrDefaultAsync(c => c.Name == model.Name),
//				Is.Null);

//			//Act
//			await currencyService.CreateCurrency(this.User1.Id, model);
//			int actualCurrencies = data.Currencies
//				.Count(c => c.OwnerId == this.User1.Id && !c.IsDeleted);

//			//Assert
//			Assert.That(model.Id, Is.Not.Null);
//			Assert.That(model.Name, Is.EqualTo("NEW"));
//			Assert.That(model.OwnerId, Is.EqualTo(this.User1.Id));
//			Assert.That(await data.Currencies.FirstOrDefaultAsync(c => c.Name == "NEW"), Is.Not.Null);
//			Assert.That(actualCurrencies, Is.EqualTo(currenciesBefore + 1));
//		}

//		[Test]
//		public async Task CreateCurrency_ShouldAddNewCurrencyWhenAnotherUserHaveTheSameCurrency()
//		{
//			//Arrange
//			var user2Currency = new Currency { Name = "NEW2", OwnerId = this.User2.Id };
//			data.Currencies.Add(user2Currency);
//			await data.SaveChangesAsync();

//			int currenciesBefore = data.Currencies.Count();
//			var model = new CurrencyViewModel { Name = user2Currency.Name };

//			//Assert
//			Assert.That(await data.Currencies.FindAsync(user2Currency.Id), Is.Not.Null);

//			//Act
//			await currencyService.CreateCurrency(this.User1.Id, model);
//			int actualCurrencies = data.Currencies.Count();

//			//Assert
//			Assert.That(model.Id, Is.Not.Null);
//			Assert.That(model.Name, Is.EqualTo(user2Currency.Name));
//			Assert.That(model.OwnerId, Is.EqualTo(this.User1.Id));
//			Assert.That(data.Currencies.Count(c => c.Name == "NEW2"), Is.EqualTo(2));
//			Assert.That(actualCurrencies, Is.EqualTo(currenciesBefore + 1));
//		}

//		[Test]
//		public async Task CreateCurrency_ShouldRecreateDeletedBeforeCurrency_WithValidParams()
//		{
//			//Arrange
//			var deletedCurrency = new Currency
//			{
//				Name = "DeletedCurrency",
//				OwnerId = this.User1.Id,
//				IsDeleted = true
//			};
//			data.Currencies.Add(deletedCurrency);
//			await data.SaveChangesAsync();
//			int countBefore = data.Currencies.Count();
//			var model = new CurrencyViewModel { Name = deletedCurrency.Name };

//			//Assert
//			Assert.That(async () =>
//			{
//				var deletedCurr = await data.Currencies.FindAsync(deletedCurrency.Id);
//				Assert.That(deletedCurr, Is.Not.Null);
//				return deletedCurr.IsDeleted;
//			}, Is.True);

//			//Act
//			await currencyService.CreateCurrency(this.User1.Id, model);
//			int countAfter = data.Currencies.Count();

//			//Assert
//			Assert.That(model.Id, Is.Not.Null);
//			Assert.That(countAfter, Is.EqualTo(countBefore));
//			Assert.That(model.OwnerId, Is.EqualTo(this.User1.Id));
//			Assert.That(model.Name, Is.EqualTo(deletedCurrency.Name));
//		}

//		[Test]
//		public void CreateCurrency_ShouldThrowException_WithExistingCurrency()
//		{
//			//Arrange
//			var model = new CurrencyViewModel { Name = this.Currency1.Name };

//			//Act & Assert
//			Assert.That(async () => await currencyService.CreateCurrency(this.User1.Id, model),
//				Throws.TypeOf<ArgumentException>()
//					.With.Message.EqualTo("Currency with the same name exist!"));
//		}

//		[Test]
//		[TestCase("A")]
//		[TestCase("NameWith11!")]
//		public void CreateCurrency_ShouldThrowException_WithInvalidName(string currencyName)
//		{
//			//Arrange
//			var model = new CurrencyViewModel { Name = currencyName };

//			//Act & Assert
//			Assert.That(async () => await currencyService.CreateCurrency(this.User1.Id, model),
//				Throws.TypeOf<ArgumentException>().With.Message
//					.EqualTo("Currency name must be between 2 and 10 characters long."));
//		}

//		[Test]
//		public async Task DeleteCurrency_ShouldDeleteCurrency_WithValidParams()
//		{
//			//Arrange
//			var newCurrency = new Currency { Name = "DEL", OwnerId = this.User1.Id };
//			data.Currencies.Add(newCurrency);
//			await data.SaveChangesAsync();

//			//Assert
//			Assert.That(await data.Currencies.FindAsync(newCurrency.Id), Is.Not.Null);
//			Assert.That(newCurrency.IsDeleted, Is.False);

//			//Act
//			await currencyService.DeleteCurrency(newCurrency.Id, this.User1.Id);

//			//Assert
//			Assert.That(newCurrency.IsDeleted, Is.True);
//		}

//		[Test]
//		public void DeleteCurrency_ShouldThrowException_WhenCurrencyNotExist()
//		{
//			//Arrange

//			//Act & Assert
//			Assert.That(async () => await currencyService.DeleteCurrency(Guid.NewGuid().ToString(), this.User1.Id),
//				Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Currency does not exist."));
//		}

//		[Test]
//		public async Task DeleteCurrency_ShouldThrowException_WhenUserIsNotOwner()
//		{
//			//Arrange
//			var newCurrency = new Currency { Name = "NOT", OwnerId = this.User2.Id };
//			data.Currencies.Add(newCurrency);
//			await data.SaveChangesAsync();

//			//Act & Assert
//			Assert.That(async () => await currencyService.DeleteCurrency(newCurrency.Id, this.User1.Id),
//				Throws.TypeOf<InvalidOperationException>()
//					.With.Message.EqualTo("Can't delete someone else Currency."));
//		}

//		[Test]
//		public async Task UserCurrencies_ShouldReturnCorrectData()
//		{
//			//Arrange
//			var expectedCategories = this.data.Currencies
//				.Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
//				.OrderBy(c => c.Name)
//				.Select(c => this.mapper.Map<CurrencyViewModel>(c))
//				.ToList();

//			//Act
//			var actualCategories = await this.currencyService.GetUserCurrencies(this.User1.Id);

//			//Assert
//			Assert.That(actualCategories, Is.Not.Null);
//			Assert.That(actualCategories.Count(), Is.EqualTo(expectedCategories.Count));
//			Assert.That(actualCategories.ElementAt(1).Id, Is.EqualTo(expectedCategories.ElementAt(1).Id));
//			Assert.That(actualCategories.ElementAt(1).Name, Is.EqualTo(expectedCategories.ElementAt(1).Name));
//		}
//	}
//}