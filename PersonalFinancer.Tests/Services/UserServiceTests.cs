using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	class UserServiceTests : UnitTestsBase
	{
		private IUsersService userService;

		[SetUp]
		public void SetUp()
		{
			this.userService = new UsersService(this.data, this.mapper);
		}

		[Test]
		public void AccountsCount_ShouldReturnAccountsCount()
		{
			//Arrange
			int expectedCount = data.Accounts.Count(a => !a.IsDeleted);

			//Act
			int actualCount = userService.GetUsersAccountsCount();

			//Assert
			Assert.That(actualCount, Is.EqualTo(expectedCount));
		}

		[Test]
		public async Task AllAccountsCardViewModel_ShouldReturnUsersAccounts_WithValidId()
		{
			//Arrange
			var expectedAccounts = await data.Accounts
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.ProjectTo<AccountCardViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			//Act
			var actualAccounts = await userService.GetUserAccounts(this.User1.Id);

			//Assert
			Assert.That(actualAccounts, Is.Not.Null);
			Assert.That(actualAccounts.Count(), Is.EqualTo(expectedAccounts.Count));
			for (int i = 0; i < expectedAccounts.Count; i++)
			{
				Assert.That(actualAccounts.ElementAt(i).Id,
					Is.EqualTo(expectedAccounts.ElementAt(i).Id));
				Assert.That(actualAccounts.ElementAt(i).Name,
					Is.EqualTo(expectedAccounts.ElementAt(i).Name));
				Assert.That(actualAccounts.ElementAt(i).Balance,
					Is.EqualTo(expectedAccounts.ElementAt(i).Balance));
			}
		}

		[Test]
		public async Task All_ShouldReturnCollectionOfAllUsers()
		{
			//Arrange
			var actual = new AllUsersViewModel();

			var expected = await data.Users
				.OrderBy(u => u.FirstName)
				.ThenBy(u => u.LastName)
				.Skip(actual.Pagination.ElementsPerPage * (actual.Pagination.Page - 1))
				.Take(actual.Pagination.ElementsPerPage)
				.ProjectTo<UserViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			//Act
			actual = await userService.GetAllUsers();

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Users.Count(), Is.EqualTo(expected.Count));
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.That(actual.Users.ElementAt(i).Id, Is.EqualTo(expected.ElementAt(i).Id));
				Assert.That(actual.Users.ElementAt(i).Email, Is.EqualTo(expected.ElementAt(i).Email));
				Assert.That(actual.Users.ElementAt(i).FirstName, Is.EqualTo(expected.ElementAt(i).FirstName));
				Assert.That(actual.Users.ElementAt(i).LastName, Is.EqualTo(expected.ElementAt(i).LastName));
			}
		}

		//[Test]
		//public async Task SetUserDashboard_ShouldReturnCorrectData_WithValidParams()
		//{
		//	//Arrange
		//	var actualDashboard = new UserDashboardViewModel()
		//	{
		//		StartDate = DateTime.UtcNow.AddMonths(-1),
		//		EndDate = DateTime.UtcNow
		//	};

		//	IEnumerable<AccountCardViewModel> expectedAccounts = await data.Accounts
		//		.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
		//		.Select(a => mapper.Map<AccountCardViewModel>(a))
		//		.ToListAsync();

		//	var expectedCashFlow = new Dictionary<string, CashFlowViewModel>();

		//	await data.Transactions
		//		.Where(t =>	t.OwnerId == this.User1.Id 
		//			&& t.CreatedOn >= actualDashboard.StartDate 
		//			&& t.CreatedOn <= actualDashboard.EndDate)
		//		.ForEachAsync(t =>
		//		{
		//			if (!expectedCashFlow.ContainsKey(t.Account.Currency.Name))
		//			{
		//				expectedCashFlow[t.Account.Currency.Name] = new CashFlowViewModel();
		//			}

		//			if (t.TransactionType == TransactionType.Income)
		//			{
		//				expectedCashFlow[t.Account.Currency.Name].Incomes += t.Amount;
		//			}
		//			else
		//			{
		//				expectedCashFlow[t.Account.Currency.Name].Expenses += t.Amount;
		//			}
		//		});

		//	var expectedLastFiveTransaction = await data.Transactions
		//		.Where(t =>
		//			t.Account.OwnerId == this.User1.Id &&
		//			t.CreatedOn >= actualDashboard.StartDate &&
		//			t.CreatedOn <= actualDashboard.EndDate)
		//		.OrderByDescending(t => t.CreatedOn)
		//		.Take(5)
		//		.Select(t => mapper.Map<TransactionTableViewModel>(t))
		//		.ToListAsync();

		//	//Act
		//	await userService.SetUserDashboard(this.User1.Id, actualDashboard);

		//	//Assert
		//	Assert.That(actualDashboard.Accounts.Count(), Is.EqualTo(expectedAccounts.Count()));
		//	for (int i = 0; i < actualDashboard.Accounts.Count(); i++)
		//	{
		//		Assert.That(actualDashboard.Accounts.ElementAt(i).Id,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Id));
		//		Assert.That(actualDashboard.Accounts.ElementAt(i).Name,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Name));
		//	}

		//	Assert.That(actualDashboard.Transactions.Count(),
		//	Is.EqualTo(expectedLastFiveTransaction.Count()));
		//	for (int i = 0; i < actualDashboard.Transactions.Count(); i++)
		//	{
		//		Assert.That(actualDashboard.Transactions.ElementAt(i).Id,
		//			Is.EqualTo(expectedLastFiveTransaction.ElementAt(i).Id));
		//		Assert.That(actualDashboard.Transactions.ElementAt(i).Amount,
		//			Is.EqualTo(expectedLastFiveTransaction.ElementAt(i).Amount));
		//	}

		//	Assert.That(actualDashboard.CurrenciesCashFlow.Count,
		//	Is.EqualTo(expectedCashFlow.Count));

		//	foreach (string expectedKey in expectedCashFlow.Keys)
		//	{
		//		Assert.That(actualDashboard.CurrenciesCashFlow.ContainsKey(expectedKey), Is.True);

		//		Assert.That(actualDashboard.CurrenciesCashFlow[expectedKey].Incomes,
		//			Is.EqualTo(expectedCashFlow[expectedKey].Incomes));

		//		Assert.That(actualDashboard.CurrenciesCashFlow[expectedKey].Expenses,
		//			Is.EqualTo(expectedCashFlow[expectedKey].Expenses));
		//	}
		//}

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

		//[Test]
		//public async Task GetUserAccountsCashFlow_ShouldReturnCorrectData_WithValidId()
		//{
		//	//Arrange
		//	DateTime startDate = DateTime.UtcNow.AddMonths(-1);
		//	DateTime endDate = DateTime.UtcNow;

		//	var expectedCashFlow = new Dictionary<string, CashFlowViewModel>();

		//	await data.Accounts
		//		.Where(a => a.OwnerId == this.User1.Id && a.Transactions
		//			.Any(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate))
		//		.Include(a => a.Currency)
		//		.Include(a => a.Transactions
		//			.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate))
		//		.ForEachAsync(a =>
		//		{
		//			if (!expectedCashFlow.ContainsKey(a.Currency.Name))
		//			{
		//				expectedCashFlow[a.Currency.Name] = new CashFlowViewModel();
		//			}

		//			decimal? income = a.Transactions?
		//				.Where(t => t.TransactionType == TransactionType.Income)
		//				.Sum(t => t.Amount);

		//			if (income != null)
		//			{
		//				expectedCashFlow[a.Currency.Name].Incomes += (decimal)income;
		//			}

		//			decimal? expense = a.Transactions?
		//				.Where(t => t.TransactionType == TransactionType.Expense)
		//				.Sum(t => t.Amount);

		//			if (expense != null)
		//			{
		//				expectedCashFlow[a.Currency.Name].Expenses += (decimal)expense;
		//			}
		//		});

		//	//Act
		//	var actualCashFlow = await accountService
		//		.GetUserAccountsCashFlow(this.User1.Id, startDate, endDate);

		//	//Assert
		//	Assert.That(actualCashFlow.Count, Is.EqualTo(expectedCashFlow.Count));

		//	foreach (string expectedKey in expectedCashFlow.Keys)
		//	{
		//		Assert.That(actualCashFlow.ContainsKey(expectedKey), Is.True);

		//		Assert.That(actualCashFlow[expectedKey].Incomes,
		//			Is.EqualTo(expectedCashFlow[expectedKey].Incomes));

		//		Assert.That(actualCashFlow[expectedKey].Expenses,
		//			Is.EqualTo(expectedCashFlow[expectedKey].Expenses));
		//	}
		//}

		
		//[Test]
		//public async Task LastFiveTransactions_ShouldReturnCorrectData_WithValidParams()
		//{
		//	//Arrange
		//	DateTime startDate = DateTime.UtcNow.AddMonths(-1);
		//	DateTime endDate = DateTime.UtcNow;

		//	IEnumerable<TransactionShortViewModel> expected = await data.Transactions
		//	.Where(t =>
		//		t.Account.OwnerId == this.User1.Id &&
		//		t.CreatedOn >= startDate &&
		//		t.CreatedOn <= endDate)
		//	.OrderByDescending(t => t.CreatedOn)
		//	.Take(5)
		//	.Select(t => mapper.Map<TransactionShortViewModel>(t))
		//	.ToListAsync();

		//	//Act
		//	var actual = await transactionService.GetUserLastFiveTransactions(this.User1.Id, startDate, endDate);

		//	//Assert
		//	Assert.That(actual.Count(),
		//	Is.EqualTo(expected.Count()));
		//	for (int i = 0; i < actual.Count(); i++)
		//	{
		//		Assert.That(actual.ElementAt(i).Id,
		//			Is.EqualTo(expected.ElementAt(i).Id));
		//		Assert.That(actual.ElementAt(i).Amount,
		//			Is.EqualTo(expected.ElementAt(i).Amount));
		//	}
		//}

		//[Test]
		//public async Task GetAllAccountsCashFlow_ShouldReturnCorrectData()
		//{
		//	//Arrange
		//	var expected = new Dictionary<string, CashFlowViewModel>();

		//	await data.Accounts
		//		.Where(a => a.Transactions.Any())
		//		.Include(a => a.Currency)
		//		.Include(a => a.Transactions)
		//		.ForEachAsync(a =>
		//		{
		//			if (!expected.ContainsKey(a.Currency.Name))
		//			{
		//				expected[a.Currency.Name] = new CashFlowViewModel();
		//			}

		//			decimal? income = a.Transactions?
		//				.Where(t => t.TransactionType == TransactionType.Income)
		//				.Sum(t => t.Amount);

		//			if (income != null)
		//			{
		//				expected[a.Currency.Name].Incomes += (decimal)income;
		//			}

		//			decimal? expense = a.Transactions?
		//				.Where(t => t.TransactionType == TransactionType.Expense)
		//				.Sum(t => t.Amount);

		//			if (expense != null)
		//			{
		//				expected[a.Currency.Name].Expenses += (decimal)expense;
		//			}
		//		});

		//	//Act
		//	var actual = await accountService.GetAllAccountsCashFlow();

		//	//Assert
		//	Assert.That(actual.Count, Is.EqualTo(expected.Count));
		//	for (int i = 0; i < expected.Count; i++)
		//	{
		//		Assert.That(actual.ElementAt(i).Key,
		//			Is.EqualTo(expected.ElementAt(i).Key));
		//		Assert.That(actual.ElementAt(i).Value.Incomes,
		//			Is.EqualTo(expected.ElementAt(i).Value.Incomes));
		//		Assert.That(actual.ElementAt(i).Value.Expenses,
		//			Is.EqualTo(expected.ElementAt(i).Value.Expenses));
		//	}
		//}
		//[Test]
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

		//[Test]
		//public async Task AccountTypesViewModel_ShouldReturnCorrectData_WithValidUserId()
		//{
		//	//Arrange
		//	IEnumerable<AccountTypeViewModel> accountTypesInDb = data.AccountTypes
		//		.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
		//		.OrderBy(a => a.Name)
		//		.Select(a => mapper.Map<AccountTypeViewModel>(a))
		//		.AsEnumerable();

		//	//Act
		//	IEnumerable<AccountTypeViewModel> actualAccountTypes = await accountTypeService
		//		.GetUserAccountTypesViewModel(this.User1.Id);

		//	//Assert
		//	Assert.That(actualAccountTypes, Is.Not.Null);
		//	Assert.That(actualAccountTypes.Count(), Is.EqualTo(accountTypesInDb.Count()));
		//	for (int i = 0; i < actualAccountTypes.Count(); i++)
		//	{
		//		Assert.That(actualAccountTypes.ElementAt(i).Id,
		//			Is.EqualTo(accountTypesInDb.ElementAt(i).Id));
		//		Assert.That(actualAccountTypes.ElementAt(i).Name,
		//			Is.EqualTo(accountTypesInDb.ElementAt(i).Name));
		//	}
		//}

		//[Test]
		//public async Task UserCategories_ShouldWorkCorrectly()
		//{
		//	//Arrange: Get first user categories where the user has custom category
		//	List<CategoryViewModel> expectedFirstUserCategories = data.Categories
		//		.Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
		//		.OrderBy(c => c.Name)
		//		.Select(c => mapper.Map<CategoryViewModel>(c))
		//		.ToList();

		//	//Arrange: Get second user categories where the user hasn't custom categories
		//	List<CategoryViewModel> expectedSecondUserCategories = data.Categories
		//		.Where(c => c.OwnerId == this.User2.Id && !c.IsDeleted)
		//		.OrderBy(c => c.Name)
		//		.Select(c => mapper.Map<CategoryViewModel>(c))
		//		.ToList();

		//	//Act: Get actual users categories
		//	IEnumerable<CategoryViewModel> actualFirstUserCategories = await categoryService
		//		.GetUserCategories(this.User1.Id);

		//	IEnumerable<CategoryViewModel> actualSecondUserCategories = await categoryService
		//		.GetUserCategories(this.User2.Id);

		//	//Assert
		//	Assert.That(actualFirstUserCategories,
		//		Is.Not.Null);

		//	Assert.That(actualFirstUserCategories.Count(),
		//		Is.EqualTo(expectedFirstUserCategories.Count));

		//	for (int i = 0; i < actualFirstUserCategories.Count(); i++)
		//	{
		//		Assert.That(actualFirstUserCategories.ElementAt(i).Id,
		//			Is.EqualTo(expectedFirstUserCategories.ElementAt(i).Id));
		//	}

		//	Assert.That(actualSecondUserCategories,
		//		Is.Not.Null);

		//	Assert.That(actualSecondUserCategories.Count(),
		//		Is.EqualTo(expectedSecondUserCategories.Count));

		//	for (int i = 0; i < actualSecondUserCategories.Count(); i++)
		//	{
		//		Assert.That(actualSecondUserCategories.ElementAt(i).Id,
		//			Is.EqualTo(expectedSecondUserCategories.ElementAt(i).Id));
		//	}
		//}
	}
}
