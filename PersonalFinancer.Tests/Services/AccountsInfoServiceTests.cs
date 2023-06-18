namespace PersonalFinancer.Tests.Services
{
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Enums;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Shared.Models;
    using static PersonalFinancer.Data.Constants.CategoryConstants;
    using static PersonalFinancer.Services.Constants.PaginationConstants;

    [TestFixture]
	internal class AccountsInfoServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<Transaction> transactionsRepo;
		private IEfRepository<Account> accountsRepo;
		private IEfRepository<Category> categoriesRepo;
		private IAccountsInfoService accountsInfoService;

		[SetUp]
		public void SetUp()
		{
			this.transactionsRepo = new EfRepository<Transaction>(this.sqlDbContext);
			this.accountsRepo = new EfRepository<Account>(this.sqlDbContext);
			this.categoriesRepo = new EfRepository<Category>(this.sqlDbContext);
			this.accountsInfoService = new AccountsInfoService(this.transactionsRepo, this.accountsRepo, this.mapper);
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
			TransactionsServiceModel actual = await this.accountsInfoService
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
			TransactionsServiceModel transactionsData = await this.accountsInfoService
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
			TransactionsServiceModel actual = await this.accountsInfoService
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
			TransactionsServiceModel transactionsData = await this.accountsInfoService
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

		[Test]
		public async Task GetAccountDetails_ShouldReturnAccountDetails_WithValidId()
		{
			//Arrange
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;
			DateTime startDateUtc = startDate.ToUniversalTime();
			DateTime endDateUtc = endDate.ToUniversalTime();

			AccountDetailsServiceModel? expected = await this.accountsRepo.All()
				.Where(a => a.Id == this.Account1User1.Id && !a.IsDeleted)
				.Select(a => new AccountDetailsServiceModel
				{
					Id = a.Id,
					Name = a.Name,
					Balance = a.Balance,
					OwnerId = this.User1.Id,
					CurrencyName = a.Currency.Name,
					AccountTypeName = a.AccountType.Name,
					StartDate = startDate,
					EndDate = endDate,
					TotalAccountTransactions = a.Transactions
						.Count(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc),
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
						.OrderByDescending(t => t.CreatedOn)
						.Take(TransactionsPerPage)
						.Select(t => new TransactionTableServiceModel
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = a.Currency.Name,
							CreatedOn = t.CreatedOn.ToLocalTime(),
							CategoryName = t.Category.Name,
							TransactionType = t.TransactionType.ToString(),
							Reference = t.Reference
						})
				})
				.FirstOrDefaultAsync();

			//Assert
			Assert.That(expected, Is.Not.Null);

			//Act
			AccountDetailsServiceModel actual = await this.accountsInfoService.GetAccountDetailsAsync(
				this.Account1User1.Id, startDate, endDate, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.Name, Is.EqualTo(expected.Name));
				Assert.That(actual.Balance, Is.EqualTo(expected.Balance));
				Assert.That(actual.CurrencyName, Is.EqualTo(expected.CurrencyName));
				Assert.That(actual.AccountTypeName, Is.EqualTo(expected.AccountTypeName));
				Assert.That(actual.OwnerId, Is.EqualTo(expected.OwnerId));
				Assert.That(actual.Transactions.Count(), Is.EqualTo(expected.Transactions.Count()));

				for (int i = 0; i < expected.Transactions.Count(); i++)
				{
					Assert.That(actual.Transactions.ElementAt(i).Id,
						Is.EqualTo(expected.Transactions.ElementAt(i).Id));
					Assert.That(actual.Transactions.ElementAt(i).Amount,
						Is.EqualTo(expected.Transactions.ElementAt(i).Amount));
					Assert.That(actual.Transactions.ElementAt(i).Reference,
						Is.EqualTo(expected.Transactions.ElementAt(i).Reference));
					Assert.That(actual.Transactions.ElementAt(i).TransactionType,
						Is.EqualTo(expected.Transactions.ElementAt(i).TransactionType));
					Assert.That(actual.Transactions.ElementAt(i).AccountCurrencyName,
						Is.EqualTo(expected.Transactions.ElementAt(i).AccountCurrencyName));
					Assert.That(actual.Transactions.ElementAt(i).CategoryName,
						Is.EqualTo(expected.Transactions.ElementAt(i).CategoryName));
					Assert.That(actual.Transactions.ElementAt(i).CreatedOn,
						Is.EqualTo(expected.Transactions.ElementAt(i).CreatedOn));
				}
			});
		}

		[Test]
		public void GetAccountDetails_ShouldThrowException_WithInvalidId()
		{
			//Arrange
			var id = Guid.NewGuid();
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountDetailsAsync(id, startDate, endDate, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountCardsData_ShouldReturnCorrectData()
		{
			//Arrange
			AccountCardServiceModel[] expectedAccounts = await this.accountsRepo.All()
				.Where(a => !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Take(AccountsPerPage)
				.ProjectTo<AccountCardServiceModel>(this.mapper.ConfigurationProvider)
				.ToArrayAsync();

			int expectedTotalAccount = await this.accountsRepo.All()
				.CountAsync(a => !a.IsDeleted);

			//Act
			UsersAccountsCardsServiceModel actual = await this.accountsInfoService.GetAccountsCardsDataAsync(1);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual, Is.Not.Null);
				Assert.That(actual.Accounts.Count(), Is.EqualTo(expectedAccounts.Length));
				Assert.That(actual.TotalUsersAccountsCount, Is.EqualTo(expectedTotalAccount));

				for (int i = 0; i < expectedAccounts.Length; i++)
				{
					Assert.That(actual.Accounts.ElementAt(i).Id,
						Is.EqualTo(expectedAccounts[i].Id));
					Assert.That(actual.Accounts.ElementAt(i).Name,
						Is.EqualTo(expectedAccounts[i].Name));
					Assert.That(actual.Accounts.ElementAt(i).Balance,
						Is.EqualTo(expectedAccounts[i].Balance));
					Assert.That(actual.Accounts.ElementAt(i).CurrencyName,
						Is.EqualTo(expectedAccounts[i].CurrencyName));
					Assert.That(actual.Accounts.ElementAt(i).OwnerId,
						Is.EqualTo(expectedAccounts[i].OwnerId));
				}
			});
		}

		[Test]
		public async Task GetCashFlowByCurrenciesAsync_ShouldReturnCorrectData()
		{
			//Arrange
			var expectedIncomes = new Dictionary<string, decimal>();
			var expectedExpenses = new Dictionary<string, decimal>();

			await this.transactionsRepo.All()
				.Include(t => t.Account)
				.ThenInclude(a => a.Currency)
				.OrderBy(t => t.Account.Currency.Name)
				.ForEachAsync(t =>
				{
					string currency = t.Account.Currency.Name;

					if (t.TransactionType == TransactionType.Income)
					{
						if (!expectedIncomes.ContainsKey(currency))
							expectedIncomes[currency] = 0;

						expectedIncomes[currency] += t.Amount;
					}
					else
					{
						if (!expectedExpenses.ContainsKey(currency))
							expectedExpenses[currency] = 0;

						expectedExpenses[currency] += t.Amount;
					}
				});

			//Act
			IEnumerable<CurrencyCashFlowServiceModel> actual =
				await this.accountsInfoService.GetCashFlowByCurrenciesAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual, Is.Not.Null);
				foreach ((string currency, decimal amount) in expectedIncomes)
				{
					Assert.That(actual.Any(c => c.Name == currency && c.Incomes == amount),
						Is.True);
				}

				foreach ((string currency, decimal amount) in expectedExpenses)
				{
					Assert.That(actual.Any(c => c.Name == currency && c.Expenses == amount),
						Is.True);
				}
			});
		}

		[Test]
		public async Task GetAccountName_ShouldReturnAccountName_WithValidId()
		{
			//Act
			string actualName = await this.accountsInfoService
				.GetAccountNameAsync(this.Account1User1.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(actualName, Is.EqualTo(this.Account1User1.Name));
		}

		[Test]
		public void GetAccountName_ShouldThrowException_WithInvalidId()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountNameAsync(invalidId, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountsCountAsync_ShouldReturnAccountsCount()
		{
			//Arrange
			int expectedCount = await this.accountsRepo.All().CountAsync(a => !a.IsDeleted);

			//Act
			int actualCount = await this.accountsInfoService.GetAccountsCountAsync();

			//Assert
			Assert.That(actualCount, Is.EqualTo(expectedCount));
		}

		[Test]
		public async Task GetAccountFormData_ShouldReturnCorrectData()
		{
			//Arrange

			//Act
			AccountFormServiceModel formData = await this.accountsInfoService.GetAccountFormDataAsync(this.Account1User1.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(formData.Name, Is.EqualTo(this.Account1User1.Name));
				Assert.That(formData.Balance, Is.EqualTo(this.Account1User1.Balance));
				Assert.That(formData.AccountTypeId, Is.EqualTo(this.Account1User1.AccountTypeId));
				Assert.That(formData.CurrencyId, Is.EqualTo(this.Account1User1.CurrencyId));
				Assert.That(formData.OwnerId, Is.EqualTo(this.Account1User1.OwnerId));
			});
		}

		[Test]
		public async Task GetAccountFormData_ShouldReturnCorrectData_WhenUserIsAdmin()
		{
			//Arrange

			//Act
			AccountFormServiceModel formData = await this.accountsInfoService.GetAccountFormDataAsync(this.Account1User1.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(formData.Name, Is.EqualTo(this.Account1User1.Name));
				Assert.That(formData.Balance, Is.EqualTo(this.Account1User1.Balance));
				Assert.That(formData.AccountTypeId, Is.EqualTo(this.Account1User1.AccountTypeId));
				Assert.That(formData.CurrencyId, Is.EqualTo(this.Account1User1.CurrencyId));
				Assert.That(formData.OwnerId, Is.EqualTo(this.Account1User1.OwnerId));
			});
		}

		[Test]
		public void GetAccountFormData_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountFormDataAsync(invalidId, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void GetAccountFormData_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountFormDataAsync(this.Account1User1.Id, this.User2.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionFormData_ShouldReturnCorrectData_WhenTransactionIsNotInitial()
		{
			//Arrange
			List<Account> orderedUserAccounts = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.ToListAsync();

			List<Category> orderedUserCategories = await this.categoriesRepo.All()
				.Where(c => c.OwnerId == this.User1.Id && !c.IsDeleted)
				.OrderBy(c => c.Name)
				.ToListAsync();

			//Act
			TransactionFormServiceModel transactionFormModel =
				await this.accountsInfoService.GetTransactionFormDataAsync(this.Transaction2User1.Id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(transactionFormModel, Is.Not.Null);
				Assert.That(transactionFormModel.AccountId, Is.EqualTo(this.Transaction2User1.AccountId));
				Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction2User1.Amount));
				Assert.That(transactionFormModel.CategoryId, Is.EqualTo(this.Transaction2User1.CategoryId));
				Assert.That(transactionFormModel.Reference, Is.EqualTo(this.Transaction2User1.Reference));
				Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction2User1.TransactionType));
				Assert.That(transactionFormModel.IsInitialBalance, Is.EqualTo(this.Transaction2User1.IsInitialBalance));
				Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(this.Transaction2User1.CreatedOn.ToLocalTime()));
				Assert.That(transactionFormModel.UserAccounts.Count(), Is.EqualTo(orderedUserAccounts.Count));
				Assert.That(transactionFormModel.UserCategories.Count(), Is.EqualTo(orderedUserCategories.Count));

				for (int i = 0; i < orderedUserAccounts.Count; i++)
				{
					Assert.That(transactionFormModel.UserAccounts.ElementAt(i).Id,
						Is.EqualTo(orderedUserAccounts.ElementAt(i).Id));
					Assert.That(transactionFormModel.UserAccounts.ElementAt(i).Name,
						Is.EqualTo(orderedUserAccounts.ElementAt(i).Name));
				}

				for (int i = 0; i < orderedUserCategories.Count; i++)
				{
					Assert.That(transactionFormModel.UserCategories.ElementAt(i).Id,
						Is.EqualTo(orderedUserCategories.ElementAt(i).Id));
					Assert.That(transactionFormModel.UserCategories.ElementAt(i).Name,
						Is.EqualTo(orderedUserCategories.ElementAt(i).Name));
				}
			});
		}

		[Test]
		public async Task GetTransactionFormData_ShouldReturnCorrectData_WhenTransactionIsInitial()
		{
			//Arrange

			//Act
			TransactionFormServiceModel transactionFormModel =
				await this.accountsInfoService.GetTransactionFormDataAsync(this.Transaction1User1.Id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(transactionFormModel, Is.Not.Null);
				Assert.That(transactionFormModel.AccountId, Is.EqualTo(this.Transaction1User1.AccountId));
				Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1User1.Amount));
				Assert.That(transactionFormModel.CategoryId, Is.EqualTo(this.Transaction1User1.CategoryId));
				Assert.That(transactionFormModel.Reference, Is.EqualTo(this.Transaction1User1.Reference));
				Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1User1.TransactionType));
				Assert.That(transactionFormModel.IsInitialBalance, Is.EqualTo(this.Transaction1User1.IsInitialBalance));
				Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(this.Transaction1User1.CreatedOn.ToLocalTime()));
				Assert.That(transactionFormModel.UserAccounts.Count(), Is.EqualTo(1));
				Assert.That(transactionFormModel.UserAccounts.First().Name, Is.EqualTo(this.Account1User1.Name));
				Assert.That(transactionFormModel.UserCategories.Count(), Is.EqualTo(1));
				Assert.That(transactionFormModel.UserCategories.First().Name, Is.EqualTo(CategoryInitialBalanceName));
			});
		}

		[Test]
		public void GetFulfilledTransactionFormModel_ShouldThrowException_WhenTransactionDoesNotExist()
		{
			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetTransactionFormDataAsync(Guid.NewGuid()),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionDetailsServiceModel transactionFormModel =
				await this.accountsInfoService.GetTransactionDetailsAsync(this.Transaction1User1.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(transactionFormModel, Is.Not.Null);
				Assert.That(transactionFormModel.Id, Is.EqualTo(this.Transaction1User1.Id));
				Assert.That(transactionFormModel.AccountName, Is.EqualTo(this.Transaction1User1.Account.Name));
				Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1User1.Amount));
				Assert.That(transactionFormModel.CategoryName, Is.EqualTo(this.Transaction1User1.Category.Name));
				Assert.That(transactionFormModel.Reference, Is.EqualTo(this.Transaction1User1.Reference));
				Assert.That(transactionFormModel.OwnerId, Is.EqualTo(this.Transaction1User1.OwnerId));
				Assert.That(transactionFormModel.AccountCurrencyName, Is.EqualTo(this.Transaction1User1.Account.Currency.Name));
				Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(this.Transaction1User1.CreatedOn.ToLocalTime()));
				Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1User1.TransactionType.ToString()));
			});
		}

		[Test]
		public async Task GetOwnerId_ShouldReturnOwnerId_WithValidAccountId()
		{
			//Act
			Guid ownerId = await this.accountsInfoService.GetOwnerIdAsync(this.Account1User1.Id);

			//Assert
			Assert.That(ownerId, Is.EqualTo(this.User1.Id));
		}

		[Test]
		public void GetOwnerId_ShouldThrowException_WhenAccountIdIsInvalid()
		{
			//Assert
			Assert.That(async () => await this.accountsInfoService.GetOwnerIdAsync(Guid.NewGuid()),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WhenUserIsAdmin()
		{
			//Act
			TransactionDetailsServiceModel transactionFormModel =
				await this.accountsInfoService.GetTransactionDetailsAsync(this.Transaction1User1.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(transactionFormModel, Is.Not.Null);
				Assert.That(transactionFormModel.Id, Is.EqualTo(this.Transaction1User1.Id));
				Assert.That(transactionFormModel.AccountName, Is.EqualTo(this.Transaction1User1.Account.Name));
				Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1User1.Amount));
				Assert.That(transactionFormModel.CategoryName, Is.EqualTo(this.Transaction1User1.Category.Name));
				Assert.That(transactionFormModel.Reference, Is.EqualTo(this.Transaction1User1.Reference));
				Assert.That(transactionFormModel.OwnerId, Is.EqualTo(this.Transaction1User1.OwnerId));
				Assert.That(transactionFormModel.AccountCurrencyName, Is.EqualTo(this.Transaction1User1.Account.Currency.Name));
				Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(this.Transaction1User1.CreatedOn.ToLocalTime()));
				Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1User1.TransactionType.ToString()));
			});
		}

		[Test]
		public void GetTransactionDetails_ShouldThrowException_WithInValidTransactionId()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetTransactionDetailsAsync(invalidId, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void GetTransactionDetails_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetTransactionDetailsAsync(this.Transaction1User1.Id, this.User2.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo("User is not transaction's owner."));
		}

		[Test]
		public async Task GetAccountShortDetails_ShouldReturnCorrectData_WithValidInput()
		{
			//Act
			AccountDetailsShortServiceModel actualData = await this.accountsInfoService.GetAccountShortDetailsAsync(this.Account1User1.Id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actualData, Is.Not.Null);
				Assert.That(actualData.Name, Is.EqualTo(this.Account1User1.Name));
				Assert.That(actualData.Balance, Is.EqualTo(this.Account1User1.Balance));
				Assert.That(actualData.CurrencyName, Is.EqualTo(this.Account1User1.Currency.Name));
			});
		}

		[Test]
		public void GetAccountShortDetails_ShouldThrowException_WithInvalidInput()
		{
			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountShortDetailsAsync(Guid.NewGuid()),
			Throws.TypeOf<InvalidOperationException>());
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
			IEnumerable<AccountCardServiceModel> actualAccounts = await this.accountsInfoService.GetUserAccountsAsync(this.User1.Id);

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
	}
}
