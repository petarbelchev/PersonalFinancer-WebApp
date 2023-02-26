namespace PersonalFinancer.Tests.Services
{
	using NUnit.Framework;

	using PersonalFinancer.Services.Currency;
	using PersonalFinancer.Services.Currency.Models;

	[TestFixture]
	class CurrencyServiceTests : UnitTestsBase
	{
		private ICurrencyService currencyService;

		[SetUp] 
		public void SetUp()
		{
			this.currencyService = new CurrencyService(this.data, this.mapper);
		}

		[Test]
		public async Task UserCurrencies_ShouldReturnCorrectData()
		{
			//Arrange
			var expectedCategories = this.data.Currencies
				.Where(c => c.UserId == null || c.UserId == this.User1.Id)
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
