namespace PersonalFinancer.Tests.Services
{
	using Microsoft.EntityFrameworkCore;
	using NUnit.Framework;

	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Currency;
	using PersonalFinancer.Services.Currency.Models;

	[TestFixture]
	class CurrencyServiceTests : UnitTestsBase
	{
		private ICurrencyService currencyService;

		[SetUp]
		public void SetUp()
		{
			this.currencyService = new CurrencyService(this.data, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task CreateCurrency_ShouldAddNewCurrency_WithValidParams()
		{
			//Arrange
			int currenciesBefore = data.Currencies
				.Count(c => (c.UserId == this.User1.Id
							|| c.UserId == null)
							&& !c.IsDeleted);

			//Assert
			Assert.That(await data.Currencies.FirstOrDefaultAsync(c => c.Name == "NEW"),
				Is.Null);

			//Act
			var actual = await currencyService.CreateCurrency(this.User1.Id, "NEW");
			int actualCurrencies = data.Currencies
				.Count(c => (c.UserId == this.User1.Id
							|| c.UserId == null)
							&& !c.IsDeleted);

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Name, Is.EqualTo("NEW"));
			Assert.That(actual.UserId, Is.EqualTo(this.User1.Id));
			Assert.That(await data.Currencies.FirstOrDefaultAsync(c => c.Name == "NEW"), Is.Not.Null);
			Assert.That(actualCurrencies, Is.EqualTo(currenciesBefore + 1));
		}

		[Test]
		public async Task CreateCurrency_ShouldAddNewCurrencyWhenAnotherUserHaveTheSameCurrency()
		{
			//Arrange
			var user2Currency = new Currency { Name = "NEW2", UserId = this.User2.Id };
			data.Currencies.Add(user2Currency);
			await data.SaveChangesAsync();

			int currenciesBefore = data.Currencies.Count();

			//Assert
			Assert.That(await data.Currencies.FindAsync(user2Currency.Id), Is.Not.Null);

			//Act
			var user1AccType = await currencyService.CreateCurrency(this.User1.Id, user2Currency.Name);
			int actualCurrencies = data.Currencies.Count();

			//Assert
			Assert.That(user1AccType, Is.Not.Null);
			Assert.That(user1AccType.Name, Is.EqualTo(user2Currency.Name));
			Assert.That(user1AccType.UserId, Is.EqualTo(this.User1.Id));
			Assert.That(data.Currencies.Count(c => c.Name == "NEW2"), Is.EqualTo(2));
			Assert.That(actualCurrencies, Is.EqualTo(currenciesBefore + 1));
		}
		
		[Test]
		public void CreateCurrency_ShouldThrowException_WithExistingCurrency()
		{
			//Act & Assert
			Assert.That(async () => await currencyService.CreateCurrency(this.User1.Id, this.Currency1.Name),
				Throws.TypeOf<InvalidOperationException>()
					.With.Message.EqualTo("Currency with the same name exist!"));
		}

		[Test]
		public async Task DeleteCurrency_ShouldDeleteCurrency_WithValidParams()
		{
			//Arrange
			var newCurrency = new Currency { Name = "DEL", UserId = this.User1.Id };
			data.Currencies.Add(newCurrency);
			await data.SaveChangesAsync();

			//Assert
			Assert.That(await data.Currencies.FindAsync(newCurrency.Id), Is.Not.Null);
			Assert.That(newCurrency.IsDeleted, Is.False);

			//Act
			await currencyService.DeleteCurrency(newCurrency.Id, this.User1.Id);

			//Assert
			Assert.That(newCurrency.IsDeleted, Is.True);
		}

		[Test]
		public void DeleteCurrency_ShouldThrowException_WhenCurrencyNotExist()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await currencyService.DeleteCurrency(Guid.NewGuid(), this.User1.Id),
				Throws.TypeOf<ArgumentNullException>()
					.With.Property("ParamName").EqualTo("Currency does not exist."));
		}

		[Test]
		public async Task DeleteCurrency_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange
			var newCurrency = new Currency { Name = "NOT", UserId = this.User2.Id };
			data.Currencies.Add(newCurrency);
			await data.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await currencyService.DeleteCurrency(newCurrency.Id, this.User1.Id),
				Throws.TypeOf<InvalidOperationException>()
					.With.Message.EqualTo("You can't delete someone else Currency."));
		}

		[Test]
		public async Task UserCurrencies_ShouldReturnCorrectData()
		{
			//Arrange
			var expectedCategories = this.data.Currencies
				.Where(c => 
					(c.UserId == null || c.UserId == this.User1.Id)
					&& !c.IsDeleted)
				.Select(c => this.mapper.Map<CurrencyViewModel>(c))
				.ToList();

			//Act
			var actualCategories = await this.currencyService.UserCurrencies(this.User1.Id);

			//Assert
			Assert.That(actualCategories, Is.Not.Null);
			Assert.That(actualCategories.Count(), Is.EqualTo(expectedCategories.Count));
			Assert.That(actualCategories.ElementAt(1).Id, Is.EqualTo(expectedCategories.ElementAt(1).Id));
			Assert.That(actualCategories.ElementAt(1).Name, Is.EqualTo(expectedCategories.ElementAt(1).Name));
		}
	}
}