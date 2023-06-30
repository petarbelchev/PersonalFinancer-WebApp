namespace PersonalFinancer.Tests.Services
{
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;
	using NUnit.Framework;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Services.User.Models;
	using System.Linq.Expressions;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	[TestFixture]
	internal class UsersServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<ApplicationUser> usersRepo;
		private IEfRepository<Category> categoriesRepo;
		private IEfRepository<Account> accountsRepo;
		private IEfRepository<AccountType> accountTypeRepo;
		private IEfRepository<Currency> currenciesRepo;
		private IEfRepository<Transaction> transactionsRepo;

		private IUsersService usersService;

		[SetUp]
		public void SetUp()
		{
			this.usersRepo = new EfRepository<ApplicationUser>(this.dbContextMock);
			this.categoriesRepo = new EfRepository<Category>(this.dbContextMock);
			this.accountsRepo = new EfRepository<Account>(this.dbContextMock);
			this.accountTypeRepo = new EfRepository<AccountType>(this.dbContextMock);
			this.currenciesRepo = new EfRepository<Currency>(this.dbContextMock);
			this.transactionsRepo = new EfRepository<Transaction>(this.dbContextMock);

			this.usersService = new UsersService(this.usersRepo, this.accountsRepo,
				this.transactionsRepo, this.categoriesRepo, this.mapperMock);
		}

		[Test]
		public async Task GetAllUsers_ShouldReturnCollectionOfAllUsers()
		{
			//Arrange
			var expected = new UsersInfoDTO
			{
				Users = await this.usersRepo.All()
					.OrderBy(u => u.FirstName)
					.ThenBy(u => u.LastName)
					.Take(UsersPerPage)
					.ProjectTo<UserInfoDTO>(this.mapperMock.ConfigurationProvider)
					.ToArrayAsync(),
				TotalUsersCount = await this.usersRepo.All().CountAsync()
			};

			//Act
			UsersInfoDTO actual = await this.usersService.GetUsersInfoAsync(1);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task GetUserAccountsAndCategoriesDropdownData_ShouldReturnCorrectData()
		{
			//Arrange
			var expected = new AccountsAndCategoriesDropdownDTO
			{
				OwnerAccounts = await this.accountsRepo.All()
					.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Select(a => this.mapperMock.Map<AccountDropdownDTO>(a))
					.ToArrayAsync(),
				OwnerCategories = await this.categoriesRepo.All()
					.Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => this.mapperMock.Map<CategoryDropdownDTO>(c))
					.ToArrayAsync()
			};

			//Act
			AccountsAndCategoriesDropdownDTO actual = await this.usersService
				.GetUserAccountsAndCategoriesDropdownDataAsync(this.User1.Id);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task GetUserAccountTypesAndCurrenciesDropdownDataAsync_ShouldReturnCorrectData()
		{
			//Arrange
			var expected = new AccountTypesAndCurrenciesDropdownDTO
			{
				OwnerAccountTypes = await this.accountTypeRepo.All()
					.Where(at => at.OwnerId == this.User1.Id && !at.IsDeleted)
					.OrderBy(at => at.Name)
					.Select(at => this.mapperMock.Map<AccountTypeDropdownDTO>(at))
					.ToArrayAsync(),
				OwnerCurrencies = await this.currenciesRepo.All()
					.Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => this.mapperMock.Map<CurrencyDropdownDTO>(c))
					.ToArrayAsync()
			};

			//Act
			AccountTypesAndCurrenciesDropdownDTO actual =
				await this.usersService.GetUserAccountTypesAndCurrenciesDropdownDataAsync(this.User1.Id);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task GetUserDashboardData_ShouldReturnCorrectData_WithValidParams()
		{
			//Arrange
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			var expected = new UserDashboardDTO
			{
				Accounts = await this.accountsRepo.All()
					.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Select(a => this.mapperMock.Map<AccountCardDTO>(a))
					.ToListAsync(),
				LastTransactions = await this.transactionsRepo.All()
					.Where(t => t.OwnerId == this.User1.Id && t.CreatedOn >= startDate && t.CreatedOn <= endDate)
					.OrderByDescending(t => t.CreatedOn)
					.Take(5)
					.Select(t => this.mapperMock.Map<TransactionTableDTO>(t))
					.ToListAsync(),
				CurrenciesCashFlow = await this.accountsRepo.All()
					.Where(a => a.OwnerId == this.User1.Id && a.Transactions.Any())
					.SelectMany(a => a.Transactions.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate))
					.GroupBy(t => t.Account.Currency.Name)
					.Select(t => new CurrencyCashFlowWithExpensesByCategoriesDTO
					{
						Name = t.Key,
						Incomes = t.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
						Expenses = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount),
						ExpensesByCategories = t.Where(t => t.TransactionType == TransactionType.Expense)
							.GroupBy(t => t.Category.Name)
							.Select(t => new CategoryExpensesDTO
							{
								CategoryName = t.Key,
								ExpensesAmount = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
							})
					})
					.OrderBy(c => c.Name)
					.ToListAsync()
			};

			//Act
			UserDashboardDTO actual = await this.usersService.GetUserDashboardDataAsync(this.User1.Id, startDate, endDate);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task GetUserTransactionsAsync_ShouldReturnCorrectViewModel_WithValidInput()
		{
			//Arrange
			var dto = new TransactionsFilterDTO
			{
				UserId = this.User1.Id,
				StartDate = DateTime.Now.AddMonths(-1),
				EndDate = DateTime.Now
			};

			Expression<Func<Transaction, bool>> filter = (t) =>
				t.OwnerId == this.User1.Id
				&& t.CreatedOn >= dto.StartDate.ToUniversalTime()
				&& t.CreatedOn <= dto.EndDate.ToUniversalTime();

			var expected = new TransactionsDTO
			{
				Transactions = await this.transactionsRepo.All()
					.Where(filter)
					.OrderByDescending(t => t.CreatedOn)
					.Take(TransactionsPerPage)
					.ProjectTo<TransactionTableDTO>(this.mapperMock.ConfigurationProvider)
					.ToArrayAsync(),
				TotalTransactionsCount = await this.transactionsRepo.All().CountAsync(filter)
			};

			//Act
			TransactionsDTO actual = await this.usersService.GetUserTransactionsAsync(dto);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task GetUserTransactionsAsync_ShouldReturnEmptyDTO_WhenUserDoesNotExist()
		{
			//Arrange
			var dto = new TransactionsFilterDTO
			{
				UserId = Guid.NewGuid(),
				StartDate = DateTime.Now.AddMonths(-1),
				EndDate = DateTime.Now
			};

			//Act
			TransactionsDTO resultDto = await this.usersService.GetUserTransactionsAsync(dto);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(resultDto, Is.Not.Null);
				Assert.That(resultDto.Transactions.Count(), Is.EqualTo(0));
				Assert.That(resultDto.TotalTransactionsCount, Is.EqualTo(0));
			});
		}

		[Test]
		public async Task UserDetails_ShouldReturnCorrectData_WithValidUserId()
		{
			//Arrange
			var expected = await this.usersRepo.All()
				.Where(u => u.Id == this.User1.Id)
				.ProjectTo<UserDetailsDTO>(this.mapperMock.ConfigurationProvider)
				.FirstAsync();

			//Act
			UserDetailsDTO actual = await this.usersService.UserDetailsAsync(this.User1.Id);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task UserFullName_ShouldReturnUsersFullName_WithValidId()
		{
			//Arrange
			string expectedFullName = $"{this.User1.FirstName} {this.User1.LastName}";

			//Act
			string actualFullName = await this.usersService.UserFullNameAsync(this.User1.Id);

			//Assert
			Assert.That(actualFullName, Is.EqualTo(expectedFullName));
		}

		[Test]
		public void UserFullName_ShouldThrowException_WithInvalidId()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.usersService.UserFullNameAsync(invalidId),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task UsersCount_ShouldReturnCorrectData()
		{
			//Arrange
			int expected = await this.usersRepo.All().CountAsync();

			//Act
			int actual = await this.usersService.UsersCountAsync();

			//Assert
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}
