//using AutoMapper.QueryableExtensions;
//using Microsoft.EntityFrameworkCore;
//using NUnit.Framework;

//using PersonalFinancer.Data.Enums;
//using PersonalFinancer.Services.Shared.Models;
//using PersonalFinancer.Services.User;
//using PersonalFinancer.Services.User.Models;

//namespace PersonalFinancer.Tests.Services
//{
//	[TestFixture]
//	class UserServiceTests : UnitTestsBase
//	{
//		private IUsersService userService;

//		[SetUp]
//		public void SetUp()
//		{
//			this.userService = new UsersService(this.data, this.mapper);
//		}

//		[Test]
//		public void AccountsCount_ShouldReturnAccountsCount()
//		{
//			//Arrange
//			int expectedCount = data.Accounts.Count(a => !a.IsDeleted);

//			//Act
//			int actualCount = userService.GetUsersAccountsCount();

//			//Assert
//			Assert.That(actualCount, Is.EqualTo(expectedCount));
//		}

//		[Test]
//		public async Task GetUserAccounts_ShouldReturnUsersAccounts_WithValidId()
//		{
//			//Arrange
//			var expectedAccounts = await data.Accounts
//				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
//				.ProjectTo<AccountCardServiceModel>(mapper.ConfigurationProvider)
//				.ToListAsync();

//			//Act
//			var actualAccounts = await userService.GetUserAccounts(this.User1.Id);

//			//Assert
//			Assert.That(actualAccounts, Is.Not.Null);
//			Assert.That(actualAccounts.Count(), Is.EqualTo(expectedAccounts.Count));
//			for (int i = 0; i < expectedAccounts.Count; i++)
//			{
//				Assert.That(actualAccounts.ElementAt(i).Id,
//					Is.EqualTo(expectedAccounts.ElementAt(i).Id));
//				Assert.That(actualAccounts.ElementAt(i).Name,
//					Is.EqualTo(expectedAccounts.ElementAt(i).Name));
//				Assert.That(actualAccounts.ElementAt(i).Balance,
//					Is.EqualTo(expectedAccounts.ElementAt(i).Balance));
//			}
//		}

//		[Test]
//		public async Task GetAllUsers_ShouldReturnCollectionOfAllUsers()
//		{
//			//Arrange
//			var actual = new AllUsersViewModel();

//			var expected = await data.Users
//				.OrderBy(u => u.FirstName)
//				.ThenBy(u => u.LastName)
//				.Skip(actual.Pagination.ElementsPerPage * (actual.Pagination.Page - 1))
//				.Take(actual.Pagination.ElementsPerPage)
//				.ProjectTo<UserViewModel>(mapper.ConfigurationProvider)
//				.ToListAsync();

//			//Act
//			actual = await userService.GetAllUsers();

//			//Assert
//			Assert.That(actual, Is.Not.Null);
//			Assert.That(actual.Users.Count(), Is.EqualTo(expected.Count));
//			for (int i = 0; i < expected.Count; i++)
//			{
//				Assert.That(actual.Users.ElementAt(i).Id, Is.EqualTo(expected.ElementAt(i).Id));
//				Assert.That(actual.Users.ElementAt(i).Email, Is.EqualTo(expected.ElementAt(i).Email));
//				Assert.That(actual.Users.ElementAt(i).FirstName, Is.EqualTo(expected.ElementAt(i).FirstName));
//				Assert.That(actual.Users.ElementAt(i).LastName, Is.EqualTo(expected.ElementAt(i).LastName));
//			}
//		}

//		[Test]
//		public async Task SetUserDashboard_ShouldReturnCorrectData_WithValidParams()
//		{
//			//Arrange
//			var actualDashboard = new UserDashboardViewModel()
//			{
//				StartDate = DateTime.UtcNow.AddMonths(-1),
//				EndDate = DateTime.UtcNow
//			};

//			var expectedAccounts = await data.Accounts
//				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
//				.Select(a => mapper.Map<AccountCardServiceModel>(a))
//				.ToListAsync();

//			var expectedCurrenciesCashFlow = await data.Transactions
//				.Where(t => t.OwnerId == this.User1.Id 
//					&& t.CreatedOn >= actualDashboard.StartDate
//					&& t.CreatedOn <= actualDashboard.EndDate)
//				.GroupBy(t => t.Account.Currency.Name)
//				.Select(t => new CurrencyCashFlowServiceModel
//				{
//					Name = t.Key,
//					Incomes = t.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
//					Expenses = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
//				})
//				.OrderBy(c => c.Name)
//				.ToListAsync();

//			var expectedLastFiveTransaction = await data.Transactions
//				.Where(t =>
//					t.Account.OwnerId == this.User1.Id &&
//					t.CreatedOn >= actualDashboard.StartDate &&
//					t.CreatedOn <= actualDashboard.EndDate)
//				.OrderByDescending(t => t.CreatedOn)
//				.Take(5)
//				.Select(t => mapper.Map<TransactionTableServiceModel>(t))
//				.ToListAsync();

//			//Act
//			await userService.GetUserDashboardData(this.User1.Id, actualDashboard);

//			//Assert
//			Assert.That(actualDashboard.Accounts.Count(), 
//				Is.EqualTo(expectedAccounts.Count()));

//			for (int i = 0; i < actualDashboard.Accounts.Count(); i++)
//			{
//				Assert.That(actualDashboard.Accounts.ElementAt(i).Id,
//					Is.EqualTo(expectedAccounts.ElementAt(i).Id));
//				Assert.That(actualDashboard.Accounts.ElementAt(i).Name,
//					Is.EqualTo(expectedAccounts.ElementAt(i).Name));
//			}

//			Assert.That(actualDashboard.Transactions.Count(),
//				Is.EqualTo(expectedLastFiveTransaction.Count()));
//			for (int i = 0; i < actualDashboard.Transactions.Count(); i++)
//			{
//				Assert.That(actualDashboard.Transactions.ElementAt(i).Id,
//					Is.EqualTo(expectedLastFiveTransaction.ElementAt(i).Id));
//				Assert.That(actualDashboard.Transactions.ElementAt(i).Amount,
//					Is.EqualTo(expectedLastFiveTransaction.ElementAt(i).Amount));
//			}

//			Assert.That(actualDashboard.CurrenciesCashFlow.Count(),
//				Is.EqualTo(expectedCurrenciesCashFlow.Count));

//			for (int i = 0; i < expectedCurrenciesCashFlow.Count; i++)
//			{
//				Assert.That(actualDashboard.CurrenciesCashFlow.ElementAt(i).Name, 
//					Is.EqualTo(expectedCurrenciesCashFlow[i].Name));

//				Assert.That(actualDashboard.CurrenciesCashFlow.ElementAt(i).Incomes,
//					Is.EqualTo(expectedCurrenciesCashFlow[i].Incomes));

//				Assert.That(actualDashboard.CurrenciesCashFlow.ElementAt(i).Expenses,
//					Is.EqualTo(expectedCurrenciesCashFlow[i].Expenses));
//			}
//		}

//		[Test]
//		public void UsersCount_ShouldReturnCorrectData()
//		{
//			//Arrange
//			int expected = data.Users.Count();

//			//Act
//			int actual = userService.UsersCount();

//			//Assert
//			Assert.That(actual, Is.EqualTo(expected));
//		}

//		[Test]
//		public async Task UserDetails_ShouldReturnCorrectData_WithValidUserId()
//		{
//			//Arrange
//			var expectedAccounts = await data.Accounts
//				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
//				.ToListAsync();

//			//Act
//			var actual = await userService.UserDetails(this.User1.Id);

//			//Assert
//			Assert.That(actual, Is.Not.Null);
//			Assert.That(actual.Id, Is.EqualTo(this.User1.Id));
//			Assert.That(actual.FirstName, Is.EqualTo(this.User1.FirstName));
//			Assert.That(actual.Email, Is.EqualTo(this.User1.Email));
//			Assert.That(actual.Accounts.Count(), Is.EqualTo(expectedAccounts.Count));
//			for (int i = 0; i < expectedAccounts.Count; i++)
//			{
//				Assert.That(actual.Accounts.ElementAt(i).Id,
//					Is.EqualTo(expectedAccounts[i].Id));
//				Assert.That(actual.Accounts.ElementAt(i).Name,
//					Is.EqualTo(expectedAccounts[i].Name));
//				Assert.That(actual.Accounts.ElementAt(i).Balance,
//					Is.EqualTo(expectedAccounts[i].Balance));
//			}
//		}

//		[Test]
//		public async Task FullName_ShouldReturnUsersFullName_WithValidId()
//		{
//			//Arrange
//			string expectedFullName = $"{this.User1.FirstName} {this.User1.LastName}";

//			//Act
//			string actualFullName = await userService.FullName(this.User1.Id);

//			//Assert
//			Assert.That(actualFullName, Is.EqualTo(expectedFullName));
//		}

//		[Test]
//		public void FullName_ShouldThrowException_WithInvalidId()
//		{
//			//Act & Assert
//			Assert.That(async () => await userService.FullName(Guid.NewGuid().ToString()),
//				Throws.TypeOf<InvalidOperationException>());
//		}


//[Test]
//		public async Task GetUserTransactionsViewModel_ShouldReturnCorrectViewModel_WithValidInput()
//		{
//			//Arrange
//			var dateFilterModel = new DateFilterModel
//			{
//				StartDate = DateTime.Now.AddMonths(-1),
//				EndDate = DateTime.Now
//			};

//			IEnumerable<Transaction> expectedTransactions = await data.Transactions
//	.Where(t => t.Account.OwnerId == User1.Id &&
//					t.CreatedOn >= dateFilterModel.StartDate &&
//					t.CreatedOn <= dateFilterModel.EndDate)
//				.OrderByDescending(t => t.CreatedOn)
//				.ToListAsync();

//			//Act
//			UserTransactionsViewModel actual =
//				await accountService.getuser(User1.Id, dateFilterModel);

//	//Assert
//			Assert.That(actual, Is.Not.Null);
//			Assert.That(actual.Transactions.Count(), Is.EqualTo(expectedTransactions.Count()));
//			for (int i = 0; i < expectedTransactions.Count(); i++)
//			{
//				Assert.That(actual.Transactions.ElementAt(i).Id,
//					Is.EqualTo(expectedTransactions.ElementAt(i).Id));
//				Assert.That(actual.Transactions.ElementAt(i).Amount,
//					Is.EqualTo(expectedTransactions.ElementAt(i).Amount));
//				Assert.That(actual.Transactions.ElementAt(i).CategoryName,
//					Is.EqualTo(expectedTransactions.ElementAt(i).Category.Name));
//				Assert.That(actual.Transactions.ElementAt(i).Refference,
//					Is.EqualTo(expectedTransactions.ElementAt(i).Refference));
//				Assert.That(actual.Transactions.ElementAt(i).TransactionType,
//					Is.EqualTo(expectedTransactions.ElementAt(i).TransactionType.ToString()));
//			}
//		}


//	}
//}
