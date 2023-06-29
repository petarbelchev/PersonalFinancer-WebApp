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
	using static PersonalFinancer.Services.Constants.PaginationConstants;

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
            this.usersRepo = new EfRepository<ApplicationUser>(this.sqlDbContext);
            this.categoriesRepo = new EfRepository<Category>(this.sqlDbContext);
            this.accountsRepo = new EfRepository<Account>(this.sqlDbContext);
            this.accountTypeRepo = new EfRepository<AccountType>(this.sqlDbContext);
            this.currenciesRepo = new EfRepository<Currency>(this.sqlDbContext);
            this.transactionsRepo = new EfRepository<Transaction>(this.sqlDbContext);

            this.usersService = new UsersService(this.usersRepo, this.accountsRepo, 
				this.transactionsRepo, this.categoriesRepo, this.mapper);
        }

		[Test]
		public async Task GetAllUsers_ShouldReturnCollectionOfAllUsers()
		{
			//Arrange
			UserInfoDTO[] expectedUsers = await this.usersRepo.All()
				.OrderBy(u => u.FirstName)
				.ThenBy(u => u.LastName)
				.Take(UsersPerPage)
				.ProjectTo<UserInfoDTO>(this.mapper.ConfigurationProvider)
				.ToArrayAsync();

			int expectedTotalCount = await this.usersRepo.All().CountAsync();

			//Act
			UsersInfoDTO actual = await this.usersService.GetUsersInfoAsync(1);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual, Is.Not.Null);
				Assert.That(actual.Users.Count(), Is.EqualTo(expectedUsers.Length));
				Assert.That(actual.TotalUsersCount, Is.EqualTo(expectedTotalCount));

				for (int i = 0; i < expectedUsers.Length; i++)
				{
					Assert.That(actual.Users.ElementAt(i).Id,
						Is.EqualTo(expectedUsers[i].Id));
					Assert.That(actual.Users.ElementAt(i).Email,
						Is.EqualTo(expectedUsers[i].Email));
					Assert.That(actual.Users.ElementAt(i).FirstName,
						Is.EqualTo(expectedUsers[i].FirstName));
					Assert.That(actual.Users.ElementAt(i).LastName,
						Is.EqualTo(expectedUsers[i].LastName));
				}
			});
		}

		[Test]
		public async Task GetUserAccountsAndCategoriesDropdownData_ShouldReturnCorrectData()
		{
			//Arrange
			AccountDropdownDTO[] expectedAccounts = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => this.mapper.Map<AccountDropdownDTO>(a))
				.ToArrayAsync();

			CategoryDropdownDTO[] expectedCategories = await this.categoriesRepo.All()
				.Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
				.OrderBy(c => c.Name)
				.Select(c => this.mapper.Map<CategoryDropdownDTO>(c))
				.ToArrayAsync();

			//Act
			AccountsAndCategoriesDropdownDTO actual =
				await this.usersService.GetUserAccountsAndCategoriesDropdownDataAsync(this.User1.Id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual, Is.Not.Null);

				for (int i = 0; i < expectedAccounts.Length; i++)
				{
					Assert.That(actual.OwnerAccounts.ElementAt(i).Id,
						Is.EqualTo(expectedAccounts[i].Id));
					Assert.That(actual.OwnerAccounts.ElementAt(i).Name,
						Is.EqualTo(expectedAccounts[i].Name));

					Assert.That(actual.OwnerCategories.ElementAt(i).Id,
						Is.EqualTo(expectedCategories[i].Id));
					Assert.That(actual.OwnerCategories.ElementAt(i).Name,
						Is.EqualTo(expectedCategories[i].Name));
				}
			});
		}

		[Test]
		public async Task GetUserAccountTypesAndCurrenciesDropdownDataAsync_ShouldReturnCorrectData()
		{
			//Arrange
			AccountTypeDropdownDTO[] expectedAccTypes = await this.accountTypeRepo.All()
				.Where(at => at.OwnerId == this.User1.Id && !at.IsDeleted)
				.OrderBy(at => at.Name)
				.Select(at => this.mapper.Map<AccountTypeDropdownDTO>(at))
				.ToArrayAsync();

			CurrencyDropdownDTO[] expectedCurrencies = await this.currenciesRepo.All()
				.Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
				.OrderBy(c => c.Name)
				.Select(c => this.mapper.Map<CurrencyDropdownDTO>(c))
				.ToArrayAsync();

			//Act
			AccountTypesAndCurrenciesDropdownDTO actual =
				await this.usersService.GetUserAccountTypesAndCurrenciesDropdownDataAsync(this.User1.Id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual, Is.Not.Null);

				for (int i = 0; i < expectedAccTypes.Length; i++)
				{
					Assert.That(actual.OwnerAccountTypes.ElementAt(i).Id,
						Is.EqualTo(expectedAccTypes[i].Id));
					Assert.That(actual.OwnerAccountTypes.ElementAt(i).Name,
						Is.EqualTo(expectedAccTypes[i].Name));

					Assert.That(actual.OwnerCurrencies.ElementAt(i).Id,
						Is.EqualTo(expectedCurrencies[i].Id));
					Assert.That(actual.OwnerCurrencies.ElementAt(i).Name,
						Is.EqualTo(expectedCurrencies[i].Name));
				}
			});
		}

		[Test]
		public async Task GetUserDashboardData_ShouldReturnCorrectData_WithValidParams()
		{
			//Arrange
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;

			List<AccountCardDTO> expectedAccounts = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => this.mapper.Map<AccountCardDTO>(a))
				.ToListAsync();

			List<CurrencyCashFlowWithExpensesByCategoriesDTO> expectedCurrenciesCashFlow = await this.transactionsRepo.All()
				.Where(t => t.OwnerId == this.User1.Id
							&& t.CreatedOn >= startDate && t.CreatedOn <= endDate)
				.GroupBy(t => t.Account.Currency.Name)
				.Select(t => new CurrencyCashFlowWithExpensesByCategoriesDTO
				{
					Name = t.Key,
					Incomes = t.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount),
					Expenses = t.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount),
					ExpensesByCategories = t.Where(t => t.TransactionType == TransactionType.Expense)
						.GroupBy(t => t.Category.Name)
						.Select(t => new CategoryExpensesDTO
						{
							CategoryName = t.Key,
							ExpensesAmount = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
						})
				})
				.OrderBy(c => c.Name)
				.ToListAsync();

			List<TransactionTableDTO> expectedLastFiveTransaction = await this.transactionsRepo.All()
				.Where(t => t.Account.OwnerId == this.User1.Id
							&& t.CreatedOn >= startDate && t.CreatedOn <= endDate)
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.Select(t => this.mapper.Map<TransactionTableDTO>(t))
				.ToListAsync();

			//Act
			UserDashboardDTO actual = await this.usersService.GetUserDashboardDataAsync(this.User1.Id, startDate, endDate);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.Accounts.Count(),
					Is.EqualTo(expectedAccounts.Count));

				for (int i = 0; i < actual.Accounts.Count(); i++)
				{
					Assert.That(actual.Accounts.ElementAt(i).Id,
						Is.EqualTo(expectedAccounts.ElementAt(i).Id));
					Assert.That(actual.Accounts.ElementAt(i).Name,
						Is.EqualTo(expectedAccounts.ElementAt(i).Name));
				}

				Assert.That(actual.LastTransactions.Count(),
					Is.EqualTo(expectedLastFiveTransaction.Count));

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
						IEnumerable<CategoryExpensesDTO> actualCategories = actual.CurrenciesCashFlow.ElementAt(y).ExpensesByCategories;
						IEnumerable<CategoryExpensesDTO> expectedCategories = expectedCurrenciesCashFlow[y].ExpensesByCategories;

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
			});
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
				&& t.CreatedOn >= dto.StartDate 
				&& t.CreatedOn <= dto.EndDate;

			TransactionTableDTO[] expectedTransactions = await this.transactionsRepo.All()
				.Where(filter)
				.OrderByDescending(t => t.CreatedOn)
				.Take(TransactionsPerPage)
				.ProjectTo<TransactionTableDTO>(this.mapper.ConfigurationProvider)
				.ToArrayAsync();

			int expectedTotalTransactions = await this.transactionsRepo.All().CountAsync(filter);

			//Act
			TransactionsDTO actual = await this.usersService.GetUserTransactionsAsync(dto);

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
			List<Account> expectedAccounts = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.ToListAsync();

			//Act
			UserDetailsDTO actual = await this.usersService.UserDetailsAsync(this.User1.Id);

			//Assert
			Assert.Multiple(() =>
			{
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
					Assert.That(actual.Accounts.ElementAt(i).CurrencyName,
						Is.EqualTo(expectedAccounts[i].Currency.Name));
					Assert.That(actual.Accounts.ElementAt(i).OwnerId,
						Is.EqualTo(expectedAccounts[i].OwnerId));
				}
			});
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
            //Act & Assert
            Assert.That(async () => await this.usersService.UserFullNameAsync(Guid.NewGuid()),
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
