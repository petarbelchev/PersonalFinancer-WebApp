namespace PersonalFinancer.Tests.Services
{
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Services.Transactions;
    using PersonalFinancer.Services.Transactions.Models;
    using static PersonalFinancer.Services.Infrastructure.Constants.PaginationConstants;

    [TestFixture]
	internal class TransactionsInfoServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<Transaction> transactionsRepo;
		private ITransactionsInfoService transactionInfoService;

		[SetUp]
		public void SetUp()
		{
			this.transactionsRepo = new EfRepository<Transaction>(this.sqlDbContext);
			this.transactionInfoService = new TransactionsInfoService(this.transactionsRepo);
		}

		[Test]
		public async Task GetUserTransactions_ShouldReturnCorrectViewModel_WithValidInput()
		{
			//Arrange
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;

			TransactionTableServiceModel[] expectedTransactions = await this.transactionsRepo.All()
				.Where(t => t.OwnerId == this.User1.Id
					&& t.CreatedOn >= startDate && t.CreatedOn <= endDate)
				.OrderByDescending(t => t.CreatedOn)
				.Take(TransactionsPerPage)
				.ProjectTo<TransactionTableServiceModel>(this.mapper.ConfigurationProvider)
				.ToArrayAsync();

			int expectedTotalTransactions = await this.transactionsRepo.All()
				.CountAsync(t => t.OwnerId == this.User1.Id
					&& t.CreatedOn >= startDate && t.CreatedOn <= endDate);

			//Act
			TransactionsServiceModel actual = await this.transactionInfoService
				.GetUserTransactionsAsync(this.User1.Id, startDate, endDate);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual, Is.Not.Null);
				Assert.That(actual.Transactions.Count(), Is.EqualTo(expectedTransactions.Length));
				Assert.That(actual.TotalTransactionsCount, Is.EqualTo(expectedTotalTransactions));

				for (int i = 0; i < expectedTransactions.Length; i++)
				{
					Assert.That(actual.Transactions.ElementAt(i).Id,
						Is.EqualTo(expectedTransactions.ElementAt(i).Id));
					Assert.That(actual.Transactions.ElementAt(i).Amount,
						Is.EqualTo(expectedTransactions.ElementAt(i).Amount));
					Assert.That(actual.Transactions.ElementAt(i).CategoryName,
						Is.EqualTo(expectedTransactions.ElementAt(i).CategoryName));
					Assert.That(actual.Transactions.ElementAt(i).Reference,
						Is.EqualTo(expectedTransactions.ElementAt(i).Reference));
					Assert.That(actual.Transactions.ElementAt(i).TransactionType,
						Is.EqualTo(expectedTransactions.ElementAt(i).TransactionType.ToString()));
				}
			});
		}

		[Test]
		public async Task GetUserTransactionsAsync_ShouldReturnEmptyCollection_WhenUserDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;
			int page = 1;

			//Act
			TransactionsServiceModel transactionsData = await this.transactionInfoService
				.GetUserTransactionsAsync(invalidId, startDate, endDate, page);

			//Act & Assert
			Assert.Multiple(() =>
			{
				Assert.That(transactionsData, Is.Not.Null);
				Assert.That(transactionsData.TotalTransactionsCount, Is.EqualTo(0));
				Assert.That(transactionsData.Transactions.Count(), Is.EqualTo(0));
				Assert.That(transactionsData.StartDate, Is.EqualTo(startDate));
				Assert.That(transactionsData.EndDate, Is.EqualTo(endDate));
			});
		}

		[Test]
		public async Task GetAccountTransactions_ShouldReturnCorrectData()
		{
			//Arrange
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;
			DateTime startDateUtc = startDate.ToUniversalTime();
			DateTime endDateUtc = endDate.ToUniversalTime();
			int page = 1;

			var expect = new TransactionsServiceModel
			{
				StartDate = startDate,
				EndDate = endDate,
				Transactions = await this.transactionsRepo.All()
					.Where(t => t.AccountId == this.Account1User1.Id && t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
					.OrderByDescending(t => t.CreatedOn)
					.Take(TransactionsPerPage)
					.Select(t => new TransactionTableServiceModel
					{
						Id = t.Id,
						Amount = t.Amount,
						CreatedOn = t.CreatedOn.ToLocalTime(),
						AccountCurrencyName = t.Account.Currency.Name,
						CategoryName = t.Category.Name + (t.Category.IsDeleted ?
							" (Deleted)"
							: string.Empty),
						Reference = t.Reference,
						TransactionType = t.TransactionType.ToString()
					})
					.ToListAsync(),

				TotalTransactionsCount = await this.transactionsRepo.All().CountAsync(t =>
					t.AccountId == this.Account1User1.Id && t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
			};

			//Act
			TransactionsServiceModel actual = await this.transactionInfoService
				.GetAccountTransactionsAsync(this.Account1User1.Id, startDate, endDate, page);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual, Is.Not.Null);
				Assert.That(actual.TotalTransactionsCount, Is.EqualTo(expect.TotalTransactionsCount));
				Assert.That(actual.Transactions, Is.Not.Null);
				Assert.That(actual.Transactions.Count(), Is.EqualTo(expect.Transactions.Count()));

				for (int i = 0; i < expect.Transactions.Count(); i++)
				{
					Assert.That(actual.Transactions.ElementAt(i).Id,
						Is.EqualTo(expect.Transactions.ElementAt(i).Id));
					Assert.That(actual.Transactions.ElementAt(i).Amount,
						Is.EqualTo(expect.Transactions.ElementAt(i).Amount));
					Assert.That(actual.Transactions.ElementAt(i).AccountCurrencyName,
						Is.EqualTo(expect.Transactions.ElementAt(i).AccountCurrencyName));
					Assert.That(actual.Transactions.ElementAt(i).TransactionType,
						Is.EqualTo(expect.Transactions.ElementAt(i).TransactionType));
					Assert.That(actual.Transactions.ElementAt(i).Reference,
						Is.EqualTo(expect.Transactions.ElementAt(i).Reference));
					Assert.That(actual.Transactions.ElementAt(i).CategoryName,
						Is.EqualTo(expect.Transactions.ElementAt(i).CategoryName));
					Assert.That(actual.Transactions.ElementAt(i).CreatedOn,
						Is.EqualTo(expect.Transactions.ElementAt(i).CreatedOn));
				}
			});
		}

		[Test]
		public async Task GetAccountTransactions_ShouldReturnEmptyCollection_WhenAccountDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;
			int page = 1;

			//Act
			TransactionsServiceModel transactionsData = await this.transactionInfoService
				.GetAccountTransactionsAsync(invalidId, startDate, endDate, page);

			//Act & Assert
			Assert.Multiple(() =>
			{
				Assert.That(transactionsData, Is.Not.Null);
				Assert.That(transactionsData.TotalTransactionsCount, Is.EqualTo(0));
				Assert.That(transactionsData.Transactions.Count(), Is.EqualTo(0));
				Assert.That(transactionsData.StartDate, Is.EqualTo(startDate));
				Assert.That(transactionsData.EndDate, Is.EqualTo(endDate));
			});
		}
	}
}
