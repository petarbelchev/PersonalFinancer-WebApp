namespace PersonalFinancer.Tests.Services
{
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Enums;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Shared.Models;
    using System.Linq.Expressions;
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
		public async Task GetAccountTransactions_ShouldReturnCorrectData()
		{
			//Arrange
			var dto = new AccountTransactionsFilterDTO
			{
				AccountId = this.Account1_User1_WithTransactions.Id,
				StartDate = DateTime.Now.AddMonths(-1),
				EndDate = DateTime.Now,
				Page = 1
			};

			Expression<Func<Transaction, bool>> filter = (t) =>
				t.AccountId == this.Account1_User1_WithTransactions.Id
				&& t.CreatedOn >= dto.StartDate.ToUniversalTime() && t.CreatedOn <= dto.EndDate.ToUniversalTime();

			var expect = new TransactionsDTO
			{
				Transactions = await this.transactionsRepo.All()
					.Where(filter)
					.OrderByDescending(t => t.CreatedOn)
					.Take(TransactionsPerPage)
					.ProjectTo<TransactionTableDTO>(this.mapper.ConfigurationProvider)
					.ToListAsync(),
				TotalTransactionsCount = await this.transactionsRepo.All()
					.CountAsync(filter)
			};

			//Act
			TransactionsDTO actual = await this.accountsInfoService.GetAccountTransactionsAsync(dto);

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
		public void GetAccountTransactions_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange
			var dto = new AccountTransactionsFilterDTO
			{
				AccountId = Guid.NewGuid(),
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow,
				Page = 1
			};

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountTransactionsAsync(dto), 
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountDetails_ShouldReturnAccountDetails_WithValidId()
		{
			//Arrange
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;
			DateTime startDateUtc = startDate.ToUniversalTime();
			DateTime endDateUtc = endDate.ToUniversalTime();

			AccountDetailsLongDTO? expected = await this.accountsRepo.All()
				.Where(a => a.Id == this.Account1_User1_WithTransactions.Id && !a.IsDeleted)
				.Select(a => new AccountDetailsLongDTO
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
						.Select(t => new TransactionTableDTO
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = a.Currency.Name,
							CreatedOn = t.CreatedOn.ToLocalTime(),
							CategoryName = t.Category.Name + (t.Category.IsDeleted ? " (Deleted)" : ""),
							TransactionType = t.TransactionType.ToString(),
							Reference = t.Reference
						})
				})
				.FirstOrDefaultAsync();

			//Assert
			Assert.That(expected, Is.Not.Null);

			//Act
			AccountDetailsLongDTO actual = await this.accountsInfoService.GetAccountDetailsAsync(
				this.Account1_User1_WithTransactions.Id, startDate, endDate, this.User1.Id, isUserAdmin: false);

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
			AccountCardDTO[] expectedAccounts = await this.accountsRepo.All()
				.Where(a => !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Take(AccountsPerPage)
				.ProjectTo<AccountCardDTO>(this.mapper.ConfigurationProvider)
				.ToArrayAsync();

			int expectedTotalAccount = await this.accountsRepo.All()
				.CountAsync(a => !a.IsDeleted);

			//Act
			AccountsCardsDTO actual = await this.accountsInfoService.GetAccountsCardsDataAsync(1);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual, Is.Not.Null);
				Assert.That(actual.Accounts.Count(), Is.EqualTo(expectedAccounts.Length));
				Assert.That(actual.TotalAccountsCount, Is.EqualTo(expectedTotalAccount));

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
			IEnumerable<CurrencyCashFlowDTO> actual =
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
				.GetAccountNameAsync(this.Account1_User1_WithTransactions.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(actualName, Is.EqualTo(this.Account1_User1_WithTransactions.Name));
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
			CreateEditAccountDTO formData = await this.accountsInfoService
				.GetAccountFormDataAsync(this.Account1_User1_WithTransactions.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(formData.Name, Is.EqualTo(this.Account1_User1_WithTransactions.Name));
				Assert.That(formData.Balance, Is.EqualTo(this.Account1_User1_WithTransactions.Balance));
				Assert.That(formData.AccountTypeId, Is.EqualTo(this.Account1_User1_WithTransactions.AccountTypeId));
				Assert.That(formData.CurrencyId, Is.EqualTo(this.Account1_User1_WithTransactions.CurrencyId));
				Assert.That(formData.OwnerId, Is.EqualTo(this.Account1_User1_WithTransactions.OwnerId));
			});
		}

		[Test]
		public async Task GetAccountFormData_ShouldReturnCorrectData_WhenUserIsAdmin()
		{
			//Arrange

			//Act
			CreateEditAccountDTO formData = await this.accountsInfoService
				.GetAccountFormDataAsync(this.Account1_User1_WithTransactions.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(formData.Name, Is.EqualTo(this.Account1_User1_WithTransactions.Name));
				Assert.That(formData.Balance, Is.EqualTo(this.Account1_User1_WithTransactions.Balance));
				Assert.That(formData.AccountTypeId, Is.EqualTo(this.Account1_User1_WithTransactions.AccountTypeId));
				Assert.That(formData.CurrencyId, Is.EqualTo(this.Account1_User1_WithTransactions.CurrencyId));
				Assert.That(formData.OwnerId, Is.EqualTo(this.Account1_User1_WithTransactions.OwnerId));
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
			Assert.That(async () => await this.accountsInfoService.GetAccountFormDataAsync(this.Account1_User1_WithTransactions.Id, this.User2.Id, isUserAdmin: false),
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
			CreateEditTransactionDTO transactionFormModel = await this.accountsInfoService
				.GetTransactionFormDataAsync(this.Transaction1_Expense_Account1_User1.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(transactionFormModel, Is.Not.Null);
				Assert.That(transactionFormModel.AccountId, Is.EqualTo(this.Transaction1_Expense_Account1_User1.AccountId));
				Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1_Expense_Account1_User1.Amount));
				Assert.That(transactionFormModel.CategoryId, Is.EqualTo(this.Transaction1_Expense_Account1_User1.CategoryId));
				Assert.That(transactionFormModel.Reference, Is.EqualTo(this.Transaction1_Expense_Account1_User1.Reference));
				Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1_Expense_Account1_User1.TransactionType));
				Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(this.Transaction1_Expense_Account1_User1.CreatedOn.ToLocalTime()));
				Assert.That(transactionFormModel.OwnerAccounts.Count(), Is.EqualTo(orderedUserAccounts.Count));
				Assert.That(transactionFormModel.OwnerCategories.Count(), Is.EqualTo(orderedUserCategories.Count));

				for (int i = 0; i < orderedUserAccounts.Count; i++)
				{
					Assert.That(transactionFormModel.OwnerAccounts.ElementAt(i).Id,
						Is.EqualTo(orderedUserAccounts.ElementAt(i).Id));
					Assert.That(transactionFormModel.OwnerAccounts.ElementAt(i).Name,
						Is.EqualTo(orderedUserAccounts.ElementAt(i).Name));
				}

				for (int i = 0; i < orderedUserCategories.Count; i++)
				{
					Assert.That(transactionFormModel.OwnerCategories.ElementAt(i).Id,
						Is.EqualTo(orderedUserCategories.ElementAt(i).Id));
					Assert.That(transactionFormModel.OwnerCategories.ElementAt(i).Name,
						Is.EqualTo(orderedUserCategories.ElementAt(i).Name));
				}
			});
		}

		[Test]
		public void GetTransactionFormData_ShouldThrowException_WhenTransactionIsInitial()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionFormDataAsync(this.InitialTransaction_Income_Account1_User1.Id, this.User1.Id, isUserAdmin: true),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void GetFulfilledTransactionFormModel_ShouldThrowException_WhenTransactionDoesNotExist()
		{
			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionFormDataAsync(Guid.NewGuid(), this.User1.Id, isUserAdmin: true),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionDetailsDTO transactionFormModel =
				await this.accountsInfoService.GetTransactionDetailsAsync(this.InitialTransaction_Income_Account1_User1.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(transactionFormModel, Is.Not.Null);
				Assert.That(transactionFormModel.Id, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Id));
				Assert.That(transactionFormModel.AccountName, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Account.Name));
				Assert.That(transactionFormModel.Amount, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Amount));
				Assert.That(transactionFormModel.CategoryName, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Category.Name));
				Assert.That(transactionFormModel.Reference, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Reference));
				Assert.That(transactionFormModel.OwnerId, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.OwnerId));
				Assert.That(transactionFormModel.AccountCurrencyName, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Account.Currency.Name));
				Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.CreatedOn.ToLocalTime()));
				Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.TransactionType.ToString()));
			});
		}

		[Test]
		public async Task GetOwnerId_ShouldReturnOwnerId_WithValidAccountId()
		{
			//Act
			Guid ownerId = await this.accountsInfoService.GetAccountOwnerIdAsync(this.Account1_User1_WithTransactions.Id);

			//Assert
			Assert.That(ownerId, Is.EqualTo(this.User1.Id));
		}

		[Test]
		public void GetOwnerId_ShouldThrowException_WhenAccountIdIsInvalid()
		{
			//Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountOwnerIdAsync(Guid.NewGuid()),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WhenUserIsAdmin()
		{
			//Act
			TransactionDetailsDTO transactionFormModel =
				await this.accountsInfoService.GetTransactionDetailsAsync(this.InitialTransaction_Income_Account1_User1.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(transactionFormModel, Is.Not.Null);
				Assert.That(transactionFormModel.Id, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Id));
				Assert.That(transactionFormModel.AccountName, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Account.Name));
				Assert.That(transactionFormModel.Amount, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Amount));
				Assert.That(transactionFormModel.CategoryName, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Category.Name));
				Assert.That(transactionFormModel.Reference, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Reference));
				Assert.That(transactionFormModel.OwnerId, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.OwnerId));
				Assert.That(transactionFormModel.AccountCurrencyName, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.Account.Currency.Name));
				Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.CreatedOn.ToLocalTime()));
				Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.TransactionType.ToString()));
				Assert.That(transactionFormModel.IsInitialBalance, Is.EqualTo(this.InitialTransaction_Income_Account1_User1.IsInitialBalance));
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
			Assert.That(async () => await this.accountsInfoService.GetTransactionDetailsAsync(this.InitialTransaction_Income_Account1_User1.Id, this.User2.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public async Task GetAccountShortDetails_ShouldReturnCorrectData_WithValidInput()
		{
			//Act
			AccountDetailsShortDTO actualData = await this.accountsInfoService.GetAccountShortDetailsAsync(this.Account1_User1_WithTransactions.Id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actualData, Is.Not.Null);
				Assert.That(actualData.Name, Is.EqualTo(this.Account1_User1_WithTransactions.Name));
				Assert.That(actualData.Balance, Is.EqualTo(this.Account1_User1_WithTransactions.Balance));
				Assert.That(actualData.CurrencyName, Is.EqualTo(this.Account1_User1_WithTransactions.Currency.Name));
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
			List<AccountCardDTO> expectedAccounts = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.ProjectTo<AccountCardDTO>(this.mapper.ConfigurationProvider)
				.ToListAsync();

			//Act
			IEnumerable<AccountCardDTO> actualAccounts = await this.accountsInfoService.GetUserAccountsCardsAsync(this.User1.Id);

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
