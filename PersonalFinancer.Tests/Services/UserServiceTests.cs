using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Data.Repositories;
using static PersonalFinancer.Data.Constants.PaginationConstants;

using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	class UserServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<ApplicationUser> usersRepo;
		private IEfRepository<Category> categoriesRepo;
		private IEfRepository<Account> accountsRepo;
		private IEfRepository<AccountType> accountTypeRepo;
		private IEfRepository<Currency> currenciesRepo;
		private IEfRepository<Transaction> transactionsRepo;
		private IUsersService userService;

		[SetUp]
		public void SetUp()
		{
			usersRepo = new EfRepository<ApplicationUser>(this.sqlDbContext);
			categoriesRepo = new EfRepository<Category>(this.sqlDbContext);
			accountsRepo = new EfRepository<Account>(this.sqlDbContext);
			accountTypeRepo = new EfRepository<AccountType>(this.sqlDbContext);
			currenciesRepo = new EfRepository<Currency>(this.sqlDbContext);
			transactionsRepo = new EfRepository<Transaction>(this.sqlDbContext);
			userService = new UsersService(usersRepo, categoriesRepo, accountsRepo, accountTypeRepo, currenciesRepo, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task FullName_ShouldReturnUsersFullName_WithValidId()
		{
			//Arrange
			string expectedFullName = $"{User1.FirstName} {User1.LastName}";

			//Act
			string actualFullName = await userService.FullName(User1.Id);

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

		[Test]
		public async Task GetAllUsers_ShouldReturnCollectionOfAllUsers()
		{
			//Arrange
			var expectedUsers = await usersRepo.All()
				.OrderBy(u => u.FirstName)
				.ThenBy(u => u.LastName)
				.Take(UsersPerPage)
				.ProjectTo<UserServiceModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			var expectedTotalCount = await usersRepo.All().CountAsync();

			//Act
			var actual = await userService.GetAllUsers(1);

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Users.Count(), Is.EqualTo(expectedUsers.Count));
			Assert.That(actual.TotalUsersCount, Is.EqualTo(expectedTotalCount));

			for (int i = 0; i < expectedUsers.Count; i++)
			{
				Assert.That(actual.Users.ElementAt(i).Id,
					Is.EqualTo(expectedUsers.ElementAt(i).Id));
				Assert.That(actual.Users.ElementAt(i).Email,
					Is.EqualTo(expectedUsers.ElementAt(i).Email));
				Assert.That(actual.Users.ElementAt(i).FirstName,
					Is.EqualTo(expectedUsers.ElementAt(i).FirstName));
				Assert.That(actual.Users.ElementAt(i).LastName,
					Is.EqualTo(expectedUsers.ElementAt(i).LastName));
			}
		}

		[Test]
		public async Task GetUserAccounts_ShouldReturnUsersAccounts_WithValidId()
		{
			//Arrange
			var expectedAccounts = await accountsRepo.All()
				.Where(a => a.OwnerId == User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.ProjectTo<AccountCardServiceModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			//Act
			var actualAccounts = await userService.GetUserAccounts(User1.Id);

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
		public async Task GetUserAccountsAndCategories_ShouldReturnCorrectData()
		{
			//Arrange
			AccountServiceModel[] expectedAccounts = await accountsRepo.All()
				.Where(a => a.OwnerId == User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => mapper.Map<AccountServiceModel>(a))
				.ToArrayAsync();

			CategoryServiceModel[] expectedCategories = await categoriesRepo.All()
				.Where(c => c.OwnerId == User1.Id && !c.IsDeleted)
				.OrderBy(c => c.Name)
				.Select(c => mapper.Map<CategoryServiceModel>(c))
				.ToArrayAsync();

			//Act
			var actual = await userService.GetUserAccountsAndCategories(User1.Id);

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.OwnerId, Is.EqualTo(User1.Id));
			Assert.That(actual.UserAccounts.Count(), Is.EqualTo(expectedAccounts.Length));
			Assert.That(actual.UserCategories.Count(), Is.EqualTo(expectedCategories.Length));

			for (int i = 0; i < expectedAccounts.Length; i++)
			{
				Assert.That(actual.UserAccounts.ElementAt(i).Id,
					Is.EqualTo(expectedAccounts[i].Id));
				Assert.That(actual.UserAccounts.ElementAt(i).Name,
					Is.EqualTo(expectedAccounts[i].Name));
			}

			for (int i = 0; i < expectedCategories.Length; i++)
			{
				Assert.That(actual.UserCategories.ElementAt(i).Id,
					Is.EqualTo(expectedCategories[i].Id));
				Assert.That(actual.UserCategories.ElementAt(i).Name,
					Is.EqualTo(expectedCategories[i].Name));
			}
		}

		[Test]
		public async Task GetUserAccountTypesAndCurrencies_ShouldReturnCorrectData()
		{
			//Arrange
			AccountTypeServiceModel[] expectedAccTypes = await accountTypeRepo.All()
				.Where(at => at.OwnerId == User1.Id && !at.IsDeleted)
				.OrderBy(at => at.Name)
				.Select(at => mapper.Map<AccountTypeServiceModel>(at))
				.ToArrayAsync();

			CurrencyServiceModel[] expectedCurrencies = await currenciesRepo.All()
				.Where(c => c.OwnerId == User1.Id && !c.IsDeleted)
				.OrderBy(c => c.Name)
				.Select(c => mapper.Map<CurrencyServiceModel>(c))
				.ToArrayAsync();

			//Act
			var actual = await userService.GetUserAccountTypesAndCurrencies(User1.Id);

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Currencies.Count(), Is.EqualTo(expectedCurrencies.Length));
			Assert.That(actual.AccountTypes.Count(), Is.EqualTo(expectedAccTypes.Length));

			for (int i = 0; i < expectedAccTypes.Length; i++)
			{
				Assert.That(actual.AccountTypes.ElementAt(i).Id,
					Is.EqualTo(expectedAccTypes[i].Id));
				Assert.That(actual.AccountTypes.ElementAt(i).Name,
					Is.EqualTo(expectedAccTypes[i].Name));
			}

			for (int i = 0; i < expectedCurrencies.Length; i++)
			{
				Assert.That(actual.Currencies.ElementAt(i).Id,
					Is.EqualTo(expectedCurrencies[i].Id));
				Assert.That(actual.Currencies.ElementAt(i).Name,
					Is.EqualTo(expectedCurrencies[i].Name));
			}
		}

		[Test]
		public async Task GetUsersAccountsCount_ShouldReturnAccountsCount()
		{
			//Arrange
			int expectedCount = await accountsRepo.All().CountAsync(a => !a.IsDeleted);

			//Act
			int actualCount = await userService.GetUsersAccountsCount();

			//Assert
			Assert.That(actualCount, Is.EqualTo(expectedCount));
		}

		[Test]
		public async Task GetUserTransactions_ShouldReturnCorrectViewModel_WithValidInput()
		{
			//Arrange
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;

			var expectedTransactions = await transactionsRepo.All()
				.Where(t => t.OwnerId == User1.Id
					&& t.CreatedOn >= startDate && t.CreatedOn <= endDate)
				.OrderByDescending(t => t.CreatedOn)
				.Take(TransactionsPerPage)
				.ProjectTo<TransactionTableServiceModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();

			int expectedTotalTransactions = await transactionsRepo.All()
				.CountAsync(t => t.OwnerId == User1.Id
					&& t.CreatedOn >= startDate && t.CreatedOn <= endDate);

			//Act
			var actual = await userService.GetUserTransactions(User1.Id, startDate, endDate);

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Transactions.Count(), Is.EqualTo(expectedTransactions.Count()));
			Assert.That(actual.TotalTransactionsCount, Is.EqualTo(expectedTotalTransactions));

			for (int i = 0; i < expectedTransactions.Count(); i++)
			{
				Assert.That(actual.Transactions.ElementAt(i).Id,
					Is.EqualTo(expectedTransactions.ElementAt(i).Id));
				Assert.That(actual.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expectedTransactions.ElementAt(i).Amount));
				Assert.That(actual.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(expectedTransactions.ElementAt(i).CategoryName));
				Assert.That(actual.Transactions.ElementAt(i).Refference,
					Is.EqualTo(expectedTransactions.ElementAt(i).Refference));
				Assert.That(actual.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(expectedTransactions.ElementAt(i).TransactionType.ToString()));
			}
		}

		[Test]
		public async Task GetUserDashboardData_ShouldReturnCorrectData_WithValidParams()
		{
			//Arrange
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;

			var expectedAccounts = await accountsRepo.All()
				.Where(a => a.OwnerId == User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => mapper.Map<AccountCardServiceModel>(a))
				.ToListAsync();

			var expectedCurrenciesCashFlow = await transactionsRepo.All()
				.Where(t => t.OwnerId == User1.Id
					&& t.CreatedOn >= startDate && t.CreatedOn <= endDate)
				.GroupBy(t => t.Account.Currency.Name)
				.Select(t => new CurrencyCashFlowServiceModel
				{
					Name = t.Key,
					Incomes = t.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount),
					Expenses = t.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount),
					ExpensesByCategories = t.Where(t => t.TransactionType == TransactionType.Expense)
						.GroupBy(t => t.Category.Name)
						.Select(t => new CategoryExpensesServiceModel
						{
							CategoryName = t.Key,
							ExpensesAmount = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
						})
				})
				.OrderBy(c => c.Name)
				.ToListAsync();

			var expectedLastFiveTransaction = await transactionsRepo.All()
				.Where(t => t.Account.OwnerId == User1.Id
					&& t.CreatedOn >= startDate && t.CreatedOn <= endDate)
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.Select(t => mapper.Map<TransactionTableServiceModel>(t))
				.ToListAsync();

			//Act
			var actual = await userService.GetUserDashboardData(User1.Id, startDate, endDate);

			//Assert
			Assert.That(actual.Accounts.Count(),
				Is.EqualTo(expectedAccounts.Count()));

			for (int i = 0; i < actual.Accounts.Count(); i++)
			{
				Assert.That(actual.Accounts.ElementAt(i).Id,
					Is.EqualTo(expectedAccounts.ElementAt(i).Id));
				Assert.That(actual.Accounts.ElementAt(i).Name,
					Is.EqualTo(expectedAccounts.ElementAt(i).Name));
			}

			Assert.That(actual.LastTransactions.Count(),
				Is.EqualTo(expectedLastFiveTransaction.Count()));

			for (int i = 0; i < actual.LastTransactions.Count(); i++)
			{
				Assert.That(actual.LastTransactions.ElementAt(i).Id,
					Is.EqualTo(expectedLastFiveTransaction.ElementAt(i).Id));
				Assert.That(actual.LastTransactions.ElementAt(i).Amount,
					Is.EqualTo(expectedLastFiveTransaction.ElementAt(i).Amount));
			}

			Assert.That(actual.CurrenciesCashFlow.Count(),
				Is.EqualTo(expectedCurrenciesCashFlow.Count));

			for (int i = 0; i < expectedCurrenciesCashFlow.Count; i++)
			{
				Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Name,
					Is.EqualTo(expectedCurrenciesCashFlow[i].Name));

				Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Incomes,
					Is.EqualTo(expectedCurrenciesCashFlow[i].Incomes));

				Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Expenses,
					Is.EqualTo(expectedCurrenciesCashFlow[i].Expenses));

				for (int y = 0; y < expectedCurrenciesCashFlow.Count; y++)
				{
					var actualCategories = actual.CurrenciesCashFlow.ElementAt(y).ExpensesByCategories;
					var expectedCategories = expectedCurrenciesCashFlow[y].ExpensesByCategories;

					Assert.That(actualCategories.Count(), Is.EqualTo(expectedCategories.Count()));

					for (int z = 0; z < expectedCategories.Count(); z++)
					{
						Assert.That(actualCategories.ElementAt(z).CategoryName,
							Is.EqualTo(expectedCategories.ElementAt(i).CategoryName));
						Assert.That(actualCategories.ElementAt(z).ExpensesAmount,
							Is.EqualTo(expectedCategories.ElementAt(i).ExpensesAmount));
					}
				}
			}
		}

		[Test]
		public async Task UserDetails_ShouldReturnCorrectData_WithValidUserId()
		{
			//Arrange
			var expectedAccounts = await accountsRepo.All()
				.Where(a => a.OwnerId == User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.ToListAsync();

			//Act
			var actual = await userService.UserDetails(User1.Id);

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Id, Is.EqualTo(User1.Id));
			Assert.That(actual.FirstName, Is.EqualTo(User1.FirstName));
			Assert.That(actual.Email, Is.EqualTo(User1.Email));
			Assert.That(actual.Accounts.Count(), Is.EqualTo(expectedAccounts.Count));

			for (int i = 0; i < expectedAccounts.Count; i++)
			{
				Assert.That(actual.Accounts.ElementAt(i).Id,
					Is.EqualTo(expectedAccounts[i].Id));
				Assert.That(actual.Accounts.ElementAt(i).Name,
					Is.EqualTo(expectedAccounts[i].Name));
				Assert.That(actual.Accounts.ElementAt(i).Balance,
					Is.EqualTo(expectedAccounts[i].Balance));
				Assert.That(actual.Accounts.ElementAt(i).CurrencyName,
					Is.EqualTo(expectedAccounts[i].Currency.Name));
				Assert.That(actual.Accounts.ElementAt(i).OwnerId,
					Is.EqualTo(expectedAccounts[i].OwnerId));
			}
		}

		[Test]
		public async Task UsersCount_ShouldReturnCorrectData()
		{
			//Arrange
			int expected = await usersRepo.All().CountAsync();

			//Act
			int actual = await userService.UsersCount();

			//Assert
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}
