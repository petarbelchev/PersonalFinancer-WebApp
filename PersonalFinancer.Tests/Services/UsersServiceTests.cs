namespace PersonalFinancer.Tests.Services
{
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;
	using MongoDB.Bson;
	using NUnit.Framework;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Services.Users.Models;
	using System.Linq.Expressions;
	using static PersonalFinancer.Common.Constants.CategoryConstants;
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
			this.usersRepo = new EfRepository<ApplicationUser>(this.dbContext);
			this.categoriesRepo = new EfRepository<Category>(this.dbContext);
			this.accountsRepo = new EfRepository<Account>(this.dbContext);
			this.accountTypeRepo = new EfRepository<AccountType>(this.dbContext);
			this.currenciesRepo = new EfRepository<Currency>(this.dbContext);
			this.transactionsRepo = new EfRepository<Transaction>(this.dbContext);

			this.usersService = new UsersService(
				this.usersRepo,
				this.accountsRepo,
				this.transactionsRepo,
				this.categoriesRepo,
				this.mapper,
				this.memoryCache);
		}

		[Test]
		public async Task GetAdminsIdsAsync_ShouldReturnAdminsIds()
		{
			//Arrange
			var expected = await this.usersRepo.All()
				.Where(u => u.IsAdmin)
				.Select(u => u.Id.ToString())
				.ToListAsync();

			//Act
			var actual = await this.usersService.GetAdminsIdsAsync();

			//Arrange
			Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
		}

		[Test]
		public async Task GetUsersInfoAsync_ShouldReturnCorrectData()
		{
			//Arrange
			var expected = new UsersInfoDTO
			{
				Users = await this.usersRepo.All()
					.OrderBy(u => u.FirstName)
					.ThenBy(u => u.LastName)
					.Take(UsersPerPage)
					.ProjectTo<UserInfoDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				TotalUsersCount = await this.usersRepo.All().CountAsync()
			};

			//Act
			UsersInfoDTO actual = await this.usersService.GetUsersInfoAsync(1);

			//Assert
			Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
		}

		[Test]
		public async Task GetUserAccountsAndCategoriesDropdownsAsync_ShouldReturnCorrectData()
		{
			//Arrange
			var expected = new AccountsAndCategoriesDropdownDTO
			{
				OwnerAccounts = await this.accountsRepo.All()
					.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Select(a => this.mapper.Map<DropdownDTO>(a))
					.ToArrayAsync(),
				OwnerCategories = await this.categoriesRepo.All()
					.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => this.mapper.Map<DropdownDTO>(c))
					.ToArrayAsync()
			};

			//Act
			AccountsAndCategoriesDropdownDTO actual = await this.usersService
				.GetUserAccountsAndCategoriesDropdownsAsync(this.mainTestUserId);

			//Assert
			Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
		}

		[Test]
		public async Task GetUserAccountsCardsAsync_ShouldReturnCorrectData()
		{
			//Arrange
			List<AccountCardDTO> expected = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.ProjectTo<AccountCardDTO>(this.mapper.ConfigurationProvider)
				.ToListAsync();

			//Act
			var actual = await this.usersService.GetUserAccountsCardsAsync(this.mainTestUserId);

			//Assert
			Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
		}

		[Test]
		public async Task GetUserAccountTypesAndCurrenciesDropdownsAsync_ShouldReturnCorrectData()
		{
			//Arrange
			var expected = new AccountTypesAndCurrenciesDropdownDTO
			{
				OwnerAccountTypes = await this.accountTypeRepo.All()
					.Where(at => at.OwnerId == this.mainTestUserId && !at.IsDeleted)
					.OrderBy(at => at.Name)
					.Select(at => this.mapper.Map<DropdownDTO>(at))
					.ToArrayAsync(),
				OwnerCurrencies = await this.currenciesRepo.All()
					.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => this.mapper.Map<DropdownDTO>(c))
					.ToArrayAsync()
			};

			//Act
			AccountTypesAndCurrenciesDropdownDTO actual =
				await this.usersService.GetUserAccountTypesAndCurrenciesDropdownsAsync(this.mainTestUserId);

			//Assert
			Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
		}

		[Test]
		public async Task GetUserDashboardDataAsync_ShouldReturnCorrectData()
		{
			//Arrange
			DateTime fromLocalTime = DateTime.UtcNow.AddMonths(-1);
			DateTime toLocalTime = DateTime.UtcNow;

			Expression<Func<Transaction, bool>> dateFilter = (t) =>
				t.CreatedOnUtc >= fromLocalTime && t.CreatedOnUtc <= toLocalTime;

			var expected = new UserDashboardDTO
			{
				FromLocalTime = fromLocalTime.ToLocalTime(),
				ToLocalTime = toLocalTime.ToLocalTime(),
				Accounts = await this.accountsRepo.All()
					.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Select(a => this.mapper.Map<AccountCardDTO>(a))
					.ToListAsync(),
				LastTransactions = await this.transactionsRepo.All()
					.Where(t => t.OwnerId == this.mainTestUserId)
					.AsQueryable()
					.Where(dateFilter)
					.OrderByDescending(t => t.CreatedOnUtc)
					.Take(5)
					.Select(t => this.mapper.Map<TransactionTableDTO>(t))
					.ToListAsync(),
				CurrenciesCashFlow = await this.accountsRepo.All()
					.Where(a => a.OwnerId == this.mainTestUserId
								&& a.Transactions.AsQueryable().Any(dateFilter))
					.SelectMany(a => a.Transactions.AsQueryable().Where(dateFilter))
					.GroupBy(t => t.Account.Currency.Name)
					.Select(t => new CurrencyCashFlowWithExpensesByCategoriesDTO
					{
						Name = t.Key,
						Incomes = t
							.Where(t => t.TransactionType == TransactionType.Income)
							.Sum(t => t.Amount),
						Expenses = t
							.Where(t => t.TransactionType == TransactionType.Expense)
							.Sum(t => t.Amount),
						ExpensesByCategories = t.Where(t => t.TransactionType == TransactionType.Expense)
							.GroupBy(t => t.Category.Name)
							.Select(t => new CategoryExpensesDTO
							{
								CategoryName = t.Key,
								ExpensesAmount = t
									.Where(t => t.TransactionType == TransactionType.Expense)
									.Sum(t => t.Amount)
							})
					})
					.OrderBy(c => c.Name)
					.ToListAsync()
			};

			//Act
			UserDashboardDTO actual = await this.usersService
				.GetUserDashboardDataAsync(this.mainTestUserId, fromLocalTime.ToLocalTime(), toLocalTime.ToLocalTime());

			//Assert
			Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
		}

		[Test]
		public async Task GetUserUsedDropdownsAsync_ShouldReturnCorrectData()
		{
			//Arrange
			var expected = await this.usersRepo.All()
				.Where(u => u.Id == this.mainTestUserId)
				.Select(u => new UserUsedDropdownsDTO
				{
					OwnerAccounts = u.Accounts
						.Where(a => !a.IsDeleted || a.Transactions.Any())
						.OrderBy(a => a.Name)
						.Select(a => this.mapper.Map<DropdownDTO>(a)),
					OwnerAccountTypes = u.AccountTypes
						.Where(at => !at.IsDeleted || at.Accounts.Any(a => !a.IsDeleted || a.Transactions.Any()))
						.OrderBy(at => at.Name)
						.Select(at => this.mapper.Map<DropdownDTO>(at)),
					OwnerCurrencies = u.Currencies
						.Where(c => !c.IsDeleted || c.Accounts.Any(a => !a.IsDeleted || a.Transactions.Any()))
						.OrderBy(c => c.Name)
						.Select(c => this.mapper.Map<DropdownDTO>(c)),
					OwnerCategories = u.Categories
						.Where(c => !c.IsDeleted || c.Transactions.Any())
						.OrderBy(c => c.Name)
						.Select(c => this.mapper.Map<DropdownDTO>(c))
						.ToList(),
				})
				.FirstAsync();

			expected.OwnerCategories.Add(await this.categoriesRepo.All()
				.Where(c => c.Id == Guid.Parse(InitialBalanceCategoryId))
				.ProjectTo<DropdownDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync());

			//Act
			var actual = await this.usersService.GetUserUsedDropdownsAsync(this.mainTestUserId);

			//Assert
			Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
		}

		[Test]
		public async Task GetUserTransactionsAsync_ShouldReturnCorrectViewModel_WithValidInput()
		{
			//Arrange
			var dto = new TransactionsFilterDTO
			{
				UserId = this.mainTestUserId,
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			Expression<Func<Transaction, bool>> filter = (t) =>
				t.OwnerId == this.mainTestUserId
				&& t.CreatedOnUtc >= dto.FromLocalTime.ToUniversalTime()
				&& t.CreatedOnUtc <= dto.ToLocalTime.ToUniversalTime();

			var expected = new TransactionsDTO
			{
				Transactions = await this.transactionsRepo.All()
					.Where(filter)
					.OrderByDescending(t => t.CreatedOnUtc)
					.Take(TransactionsPerPage)
					.ProjectTo<TransactionTableDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				TotalTransactionsCount = await this.transactionsRepo.All().CountAsync(filter)
			};

			//Act
			TransactionsDTO actual = await this.usersService.GetUserTransactionsAsync(dto);

			//Assert
			Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
		}

		[Test]
		public async Task GetUserTransactionsAsync_ShouldReturnEmptyDTO_WhenUserDoesNotExist()
		{
			//Arrange
			var dto = new TransactionsFilterDTO
			{
				UserId = Guid.NewGuid(),
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
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
		public async Task UserDetailsAsync_ShouldReturnCorrectData_WithValidUserId()
		{
			//Arrange
			UserDetailsDTO expected = await this.usersRepo.All()
				.Where(u => u.Id == this.mainTestUserId)
				.ProjectTo<UserDetailsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			//Act
			UserDetailsDTO actual = await this.usersService.UserDetailsAsync(this.mainTestUserId);

			//Assert
			Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
		}

		[Test]
		public async Task UserFullNameAsync_ShouldReturnUsersFullName_WithValidId()
		{
			//Arrange
			ApplicationUser? testUser = await this.dbContext.Users.FindAsync(this.mainTestUserId);
			string expectedFullName = $"{testUser!.FirstName} {testUser.LastName}";

			//Act
			string actualFullName = await this.usersService.UserFullNameAsync(this.mainTestUserId);

			//Assert
			Assert.That(actualFullName, Is.EqualTo(expectedFullName));
		}

		[Test]
		public void UserFullNameAsync_ShouldThrowInvalidOperationException_WithInvalidId()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.usersService.UserFullNameAsync(invalidId),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task UsersCountAsync_ShouldReturnCorrectData()
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
