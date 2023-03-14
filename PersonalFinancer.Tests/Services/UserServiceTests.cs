using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	class UserServiceTests : UnitTestsBase
	{
		private IUserService userService;
		private IAccountService accountService;
		private ITransactionsService transactionsService;
		private ICategoryService categoryService;

		[SetUp]
		public void SetUp()
		{
			this.categoryService = new CategoryService(this.data, this.mapper, this.memoryCache);
			this.transactionsService = new TransactionsService(this.data, this.mapper);
			this.accountService = new AccountService(this.data, this.mapper, this.transactionsService, this.categoryService, this.memoryCache);
			this.userService = new UserService(this.data, this.accountService, this.transactionsService, this.mapper);
		}

		[Test]
		public async Task All_ShouldReturnCollectionOfAllUsers()
		{
			//Arrange
			var expected = await data.Users
				.ProjectTo<UserViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			//Act
			var actual = await userService.GetAllUsers();

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Count(), Is.EqualTo(expected.Count));
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.That(actual.ElementAt(i).Id, Is.EqualTo(expected.ElementAt(i).Id));
				Assert.That(actual.ElementAt(i).Email, Is.EqualTo(expected.ElementAt(i).Email));
				Assert.That(actual.ElementAt(i).FirstName, Is.EqualTo(expected.ElementAt(i).FirstName));
				Assert.That(actual.ElementAt(i).LastName, Is.EqualTo(expected.ElementAt(i).LastName));
			}
		}

		[Test]
		public async Task GetUserDashboard_ShouldReturnCorrectData_WithValidParams()
		{
			//Arrange
			var actualDashboard = new HomeIndexViewModel()
			{
				Dates = new DateFilterModel
				{
					StartDate = DateTime.UtcNow.AddMonths(-1),
					EndDate = DateTime.UtcNow
				}
			};

			IEnumerable<AccountCardViewModel> expectedAccounts = await data.Accounts
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.Select(a => mapper.Map<AccountCardViewModel>(a))
				.ToListAsync();

			var expectedCashFlow = new Dictionary<string, CashFlowViewModel>();

			await data.Accounts
				.Where(a => a.OwnerId == this.User1.Id && a.Transactions
					.Any(t => t.CreatedOn >= actualDashboard.Dates.StartDate && t.CreatedOn <= actualDashboard.Dates.EndDate))
				.Include(a => a.Currency)
				.Include(a => a.Transactions
					.Where(t => t.CreatedOn >= actualDashboard.Dates.StartDate && t.CreatedOn <= actualDashboard.Dates.EndDate))
				.ForEachAsync(a =>
				{
					if (!expectedCashFlow.ContainsKey(a.Currency.Name))
					{
						expectedCashFlow[a.Currency.Name] = new CashFlowViewModel();
					}

					decimal? income = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount);

					if (income != null)
					{
						expectedCashFlow[a.Currency.Name].Income += (decimal)income;
					}

					decimal? expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						expectedCashFlow[a.Currency.Name].Expence += (decimal)expense;
					}
				});

			var expectedLastFiveTransaction = await data.Transactions
				.Where(t =>
					t.Account.OwnerId == this.User1.Id &&
					t.CreatedOn >= actualDashboard.Dates.StartDate &&
					t.CreatedOn <= actualDashboard.Dates.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.Select(t => mapper.Map<TransactionShortViewModel>(t))
				.ToListAsync();

			//Act
			await userService.GetUserDashboard(this.User1.Id, actualDashboard);

			//Assert
			Assert.That(actualDashboard.Accounts.Count(), Is.EqualTo(expectedAccounts.Count()));
			for (int i = 0; i < actualDashboard.Accounts.Count(); i++)
			{
				Assert.That(actualDashboard.Accounts.ElementAt(i).Id,
					Is.EqualTo(expectedAccounts.ElementAt(i).Id));
				Assert.That(actualDashboard.Accounts.ElementAt(i).Name,
					Is.EqualTo(expectedAccounts.ElementAt(i).Name));
			}

			Assert.That(actualDashboard.LastTransactions.Count(),
			Is.EqualTo(expectedLastFiveTransaction.Count()));
			for (int i = 0; i < actualDashboard.LastTransactions.Count(); i++)
			{
				Assert.That(actualDashboard.LastTransactions.ElementAt(i).Id,
					Is.EqualTo(expectedLastFiveTransaction.ElementAt(i).Id));
				Assert.That(actualDashboard.LastTransactions.ElementAt(i).Amount,
					Is.EqualTo(expectedLastFiveTransaction.ElementAt(i).Amount));
			}

			Assert.That(actualDashboard.CurrenciesCashFlow.Count,
			Is.EqualTo(expectedCashFlow.Count));

			foreach (string expectedKey in expectedCashFlow.Keys)
			{
				Assert.That(actualDashboard.CurrenciesCashFlow.ContainsKey(expectedKey), Is.True);

				Assert.That(actualDashboard.CurrenciesCashFlow[expectedKey].Income,
					Is.EqualTo(expectedCashFlow[expectedKey].Income));

				Assert.That(actualDashboard.CurrenciesCashFlow[expectedKey].Expence,
					Is.EqualTo(expectedCashFlow[expectedKey].Expence));
			}
		}

		[Test]
		public void GetUserDashboard_ShouldThrowException_WithInvalidDates()
		{
			//Arrange
			HomeIndexViewModel dashboardModel = new HomeIndexViewModel
			{
				Dates = new DateFilterModel
				{
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddMonths(-1)
				}
			};

			//Act & Assert
			Assert.That(async () => await userService.GetUserDashboard(this.User1.Id, dashboardModel),
				Throws.TypeOf<ArgumentException>().With.Message
					.EqualTo("Start Date must be before End Date."));
		}

		[Test]
		public void UsersCount_ShouldReturnCorrectData()
		{
			//Arrange
			int expected = data.Users.Count();

			//Act
			int actual = userService.UsersCount();

			//Assert
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public async Task UserDetails_ShouldReturnCorrectData_WithValidUserId()
		{
			//Arrange
			var expectedAccounts = await data.Accounts
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.ToListAsync();

			//Act
			var actual = await userService.UserDetails(this.User1.Id);

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Id, Is.EqualTo(this.User1.Id));
			Assert.That(actual.FirstName, Is.EqualTo(this.User1.FirstName));
			Assert.That(actual.Email, Is.EqualTo(this.User1.Email));
			Assert.That(actual.Accounts.Count(), Is.EqualTo(expectedAccounts.Count));
			for (int i = 0; i < expectedAccounts.Count; i++)
			{
				Assert.That(actual.Accounts.ElementAt(i).Id,
					Is.EqualTo(expectedAccounts[i].Id));
				Assert.That(actual.Accounts.ElementAt(i).Name,
					Is.EqualTo(expectedAccounts[i].Name));
				Assert.That(actual.Accounts.ElementAt(i).Balance,
					Is.EqualTo(expectedAccounts[i].Balance));
			}
		}

		[Test]
		public async Task FullName_ShouldReturnUsersFullName_WithValidId()
		{
			//Arrange
			string expectedFullName = $"{this.User1.FirstName} {this.User1.LastName}";

			//Act
			string actualFullName = await userService.FullName(this.User1.Id);

			//Assert
			Assert.That(actualFullName, Is.EqualTo(expectedFullName));
		}

		[Test]
		public void FullName_ShouldThrowException_WithInvalidId()
		{
			//Act & Assert
			Assert.That(async () => await userService.FullName(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}
	}
}
