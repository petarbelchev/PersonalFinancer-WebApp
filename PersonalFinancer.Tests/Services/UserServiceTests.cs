﻿namespace PersonalFinancer.Tests.Services
{
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Enums;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Services.User.Models;
    using static PersonalFinancer.Services.Infrastructure.Constants.PaginationConstants;

    [TestFixture]
    internal class UserServiceTests : ServicesUnitTestsBase
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
            this.usersRepo = new EfRepository<ApplicationUser>(this.sqlDbContext);
            this.categoriesRepo = new EfRepository<Category>(this.sqlDbContext);
            this.accountsRepo = new EfRepository<Account>(this.sqlDbContext);
            this.accountTypeRepo = new EfRepository<AccountType>(this.sqlDbContext);
            this.currenciesRepo = new EfRepository<Currency>(this.sqlDbContext);
            this.transactionsRepo = new EfRepository<Transaction>(this.sqlDbContext);
            this.userService = new UsersService(this.usersRepo, this.categoriesRepo, this.accountsRepo, this.accountTypeRepo, this.currenciesRepo, this.mapper, this.memoryCache);
        }

        [Test]
        public async Task UserFullName_ShouldReturnUsersFullName_WithValidId()
        {
            //Arrange
            string expectedFullName = $"{this.User1.FirstName} {this.User1.LastName}";

            //Act
            string actualFullName = await this.userService.UserFullName(this.User1.Id);

            //Assert
            Assert.That(actualFullName, Is.EqualTo(expectedFullName));
        }

        [Test]
        public void UserFullName_ShouldThrowException_WithInvalidId()
        {
            //Act & Assert
            Assert.That(async () => await this.userService.UserFullName(Guid.NewGuid()),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task GetAllUsers_ShouldReturnCollectionOfAllUsers()
        {
            //Arrange
            List<UserServiceModel> expectedUsers = await this.usersRepo.All()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Take(UsersPerPage)
                .ProjectTo<UserServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();

            int expectedTotalCount = await this.usersRepo.All().CountAsync();

            //Act
            UsersServiceModel actual = await this.userService.GetAllUsers(1);

            //Assert
            Assert.Multiple(() =>
            {
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
            });
        }

        [Test]
        public async Task GetUserAccounts_ShouldReturnUsersAccounts_WithValidId()
        {
            //Arrange
            List<AccountCardServiceModel> expectedAccounts = await this.accountsRepo.All()
                .Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
                .OrderBy(a => a.Name)
                .ProjectTo<AccountCardServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();

            //Act
            IEnumerable<AccountCardServiceModel> actualAccounts = await this.userService.GetUserAccounts(this.User1.Id);

            //Assert
            Assert.Multiple(() =>
            {
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
            });
        }

        [Test]
        public async Task GetUserAccountsAndCategories_ShouldReturnCorrectData()
        {
            //Arrange
            AccountServiceModel[] expectedAccounts = await this.accountsRepo.All()
                .Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
                .OrderBy(a => a.Name)
                .Select(a => this.mapper.Map<AccountServiceModel>(a))
                .ToArrayAsync();

            CategoryServiceModel[] expectedCategories = await this.categoriesRepo.All()
                .Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .Select(c => this.mapper.Map<CategoryServiceModel>(c))
                .ToArrayAsync();

            //Act
            UserAccountsAndCategoriesServiceModel actual = await this.userService.GetUserAccountsAndCategories(this.User1.Id);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.OwnerId, Is.EqualTo(this.User1.Id));
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
            });
        }

        [Test]
        public async Task GetUserAccountTypesAndCurrencies_ShouldReturnCorrectData()
        {
            //Arrange
            AccountTypeServiceModel[] expectedAccTypes = await this.accountTypeRepo.All()
                .Where(at => at.OwnerId == this.User1.Id && !at.IsDeleted)
                .OrderBy(at => at.Name)
                .Select(at => this.mapper.Map<AccountTypeServiceModel>(at))
                .ToArrayAsync();

            CurrencyServiceModel[] expectedCurrencies = await this.currenciesRepo.All()
                .Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .Select(c => this.mapper.Map<CurrencyServiceModel>(c))
                .ToArrayAsync();

            //Act
            UserAccountTypesAndCurrenciesServiceModel actual = await this.userService.GetUserAccountTypesAndCurrencies(this.User1.Id);

            //Assert
            Assert.Multiple(() =>
            {
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
            });
        }

        [Test]
        public async Task GetUsersAccountsCount_ShouldReturnAccountsCount()
        {
            //Arrange
            int expectedCount = await this.accountsRepo.All().CountAsync(a => !a.IsDeleted);

            //Act
            int actualCount = await this.userService.GetUsersAccountsCount();

            //Assert
            Assert.That(actualCount, Is.EqualTo(expectedCount));
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
            TransactionsServiceModel actual = await this.userService.GetUserTransactions(this.User1.Id, startDate, endDate);

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
                    Assert.That(actual.Transactions.ElementAt(i).Refference,
                        Is.EqualTo(expectedTransactions.ElementAt(i).Refference));
                    Assert.That(actual.Transactions.ElementAt(i).TransactionType,
                        Is.EqualTo(expectedTransactions.ElementAt(i).TransactionType.ToString()));
                }
            });
        }

        [Test]
        public async Task GetUserDashboardData_ShouldReturnCorrectData_WithValidParams()
        {
            //Arrange
            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now;

            List<AccountCardServiceModel> expectedAccounts = await this.accountsRepo.All()
                .Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
                .OrderBy(a => a.Name)
                .Select(a => this.mapper.Map<AccountCardServiceModel>(a))
                .ToListAsync();

            List<CurrencyCashFlowServiceModel> expectedCurrenciesCashFlow = await this.transactionsRepo.All()
                .Where(t => t.OwnerId == this.User1.Id
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

            List<TransactionTableServiceModel> expectedLastFiveTransaction = await this.transactionsRepo.All()
                .Where(t => t.Account.OwnerId == this.User1.Id
                    && t.CreatedOn >= startDate && t.CreatedOn <= endDate)
                .OrderByDescending(t => t.CreatedOn)
                .Take(5)
                .Select(t => this.mapper.Map<TransactionTableServiceModel>(t))
                .ToListAsync();

            //Act
            UserDashboardServiceModel actual = await this.userService.GetUserDashboardData(this.User1.Id, startDate, endDate);

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
                        IEnumerable<CategoryExpensesServiceModel> actualCategories = actual.CurrenciesCashFlow.ElementAt(y).ExpensesByCategories;
                        IEnumerable<CategoryExpensesServiceModel> expectedCategories = expectedCurrenciesCashFlow[y].ExpensesByCategories;

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
        public async Task UserDetails_ShouldReturnCorrectData_WithValidUserId()
        {
            //Arrange
            List<Account> expectedAccounts = await this.accountsRepo.All()
                .Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
                .OrderBy(a => a.Name)
                .ToListAsync();

            //Act
            UserDetailsServiceModel actual = await this.userService.UserDetails(this.User1.Id);

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
        public async Task UsersCount_ShouldReturnCorrectData()
        {
            //Arrange
            int expected = await this.usersRepo.All().CountAsync();

            //Act
            int actual = await this.userService.UsersCount();

            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
