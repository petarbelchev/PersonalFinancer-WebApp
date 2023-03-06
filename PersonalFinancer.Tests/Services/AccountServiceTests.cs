namespace PersonalFinancer.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;

    using PersonalFinancer.Data.Enums;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Category;
    using PersonalFinancer.Services.Transactions;

    [TestFixture]
	class AccountServiceTests : UnitTestsBase
	{
		private IAccountService accountService;
		private ITransactionsService transactionsService;
		private ICategoryService categoryService;

		[SetUp]
		public void SetUp()
		{
			this.categoryService = new CategoryService(this.data, this.mapper, this.memoryCache);
			this.transactionsService = new TransactionsService(this.data, this.mapper);
			this.accountService = new AccountService(this.data, this.mapper, this.transactionsService, this.categoryService, this.memoryCache);
		}

		[Test]
		public async Task AllAccountsDropdownViewModel_ShouldReturnCorrectData_WithValidUserId()
		{
			//Arrange
			IEnumerable<AccountDropdownViewModel> expectedAccounts = data.Accounts
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.AsEnumerable();

			//Act
			IEnumerable<AccountDropdownViewModel> actualAccounts = await accountService
				.AllAccountsDropdownViewModel(this.User1.Id);

			//Assert
			Assert.That(actualAccounts, Is.Not.Null);
			Assert.That(actualAccounts.Count(), Is.EqualTo(expectedAccounts.Count()));

			for (int i = 0; i < actualAccounts.Count(); i++)
			{
				Assert.That(actualAccounts.ElementAt(i).Id,
					Is.EqualTo(expectedAccounts.ElementAt(i).Id));
				Assert.That(actualAccounts.ElementAt(i).Name,
					Is.EqualTo(expectedAccounts.ElementAt(i).Name));
			}
		}

		[Test]
		public async Task AccountDropdownViewModel_ShouldReturnAccount_WithValidId()
		{
			//Act
			AccountDropdownViewModel? actualAccount = await accountService
				.AccountDropdownViewModel(this.Account1User1.Id);

			//Assert
			Assert.That(actualAccount, Is.Not.Null);
			Assert.That(actualAccount.Id, Is.EqualTo(this.Account1User1.Id));
			Assert.That(actualAccount.Name, Is.EqualTo(this.Account1User1.Name));
		}

		[Test]
		public async Task AccountDropdownViewModel_ShouldReturnNull_WithInvalidId()
		{
			//Act
			AccountDropdownViewModel? actualAccount = await accountService
				.AccountDropdownViewModel(Guid.NewGuid());

			//Assert
			Assert.That(actualAccount, Is.Null);
		}

		[Test]
		public async Task AccountDetailsViewModel_ShouldReturnAccountDetails_WithValidId()
		{
			//Arrange
			AccountDetailsViewModel expectedAccount = await data.Accounts
				.Where(a => a.Id == this.Account1User1.Id)
				.Select(a => mapper.Map<AccountDetailsViewModel>(a))
				.FirstAsync();

			//Act
			AccountDetailsViewModel? actualAccount = await accountService
				.AccountDetailsViewModel(this.Account1User1.Id);

			//Assert
			Assert.That(actualAccount, Is.Not.Null);
			Assert.That(actualAccount.Id, Is.EqualTo(expectedAccount.Id));
			Assert.That(actualAccount.Name, Is.EqualTo(expectedAccount.Name));
			Assert.That(actualAccount.Balance, Is.EqualTo(expectedAccount.Balance));
			Assert.That(actualAccount.Transactions.Count(), Is.EqualTo(expectedAccount.Transactions.Count()));

			for (int i = 0; i < expectedAccount.Transactions.Count(); i++)
			{
				Assert.That(actualAccount.Transactions.ElementAt(i).Id,
					Is.EqualTo(expectedAccount.Transactions.ElementAt(i).Id));
				Assert.That(actualAccount.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expectedAccount.Transactions.ElementAt(i).Amount));
			}
		}

		[Test]
		public async Task AccountDetailsViewModel_ShouldReturnNull_WithInvalidId()
		{
			//Arrange & Act
			AccountDetailsViewModel? account = await accountService
				.AccountDetailsViewModel(Guid.NewGuid());

			//Assert
			Assert.That(account, Is.Null);
		}

		[Test]
		public async Task CreateAccount_ShouldAddNewAccountAndTransaction_WithValidInput()
		{
			//Arrange
			AccountFormModel newAccountModel = new AccountFormModel
			{
				Name = "AccountWithNonZeroBalance",
				AccountTypeId = this.AccountType1.Id,
				Balance = 100,
				CurrencyId = this.Currency1.Id
			};

			int accountsCountBefore = data.Accounts.Count();

			//Act
			Guid newAccountId = await accountService.CreateAccount(this.User1.Id, newAccountModel);
			Account? newAccount = data.Accounts
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstOrDefault();

			//Assert
			Assert.That(data.Accounts.Count(), Is.EqualTo(accountsCountBefore + 1));
			Assert.That(newAccount, Is.Not.Null);
			Assert.That(newAccount.Name, Is.EqualTo(newAccountModel.Name));
			Assert.That(newAccount.Balance, Is.EqualTo(newAccountModel.Balance));
			Assert.That(newAccount.CurrencyId, Is.EqualTo(newAccountModel.CurrencyId));
			Assert.That(newAccount.AccountTypeId, Is.EqualTo(newAccountModel.AccountTypeId));
			Assert.That(newAccount.Transactions.Count, Is.EqualTo(1));
			Assert.That(newAccount.Transactions.First().CategoryId, Is.EqualTo(this.Category1.Id));
		}

		[Test]
		public async Task CreateAccount_ShouldAddNewAccountWithoutTransaction_WithValidInput()
		{
			//Arrange
			AccountFormModel newAccountModel = new AccountFormModel
			{
				Name = "AccountWithZeroBalance",
				AccountTypeId = this.AccountType1.Id,
				Balance = 0,
				CurrencyId = this.Currency1.Id
			};

			int accountsCountBefore = data.Accounts.Count();

			//Act
			Guid newAccountId = await accountService.CreateAccount(this.User1.Id, newAccountModel);
			Account? newAccount = data.Accounts
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstOrDefault();

			//Assert
			Assert.That(data.Accounts.Count(), Is.EqualTo(accountsCountBefore + 1));
			Assert.That(newAccount, Is.Not.Null);
			Assert.That(newAccount.Name, Is.EqualTo(newAccountModel.Name));
			Assert.That(newAccount.Balance, Is.EqualTo(newAccountModel.Balance));
			Assert.That(newAccount.CurrencyId, Is.EqualTo(newAccountModel.CurrencyId));
			Assert.That(newAccount.AccountTypeId, Is.EqualTo(newAccountModel.AccountTypeId));
			Assert.That(newAccount.Transactions.Any(), Is.False);
		}

		[Test]
		public async Task IsAccountOwner_ShouldReturnTrue_WhenUserIsOwner()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountOwner(this.User1.Id, this.Account1User1.Id);

			//Assert
			Assert.That(response, Is.True);
		}

		[Test]
		public async Task IsAccountOwner_ShouldReturnFalse_WhenUserIsNotOwner()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountOwner(this.User2.Id, this.Account1User1.Id);

			//Assert
			Assert.That(response, Is.False);
		}

		[Test]
		public void IsAccountOwner_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await accountService.IsAccountOwner(this.User1.Id, Guid.NewGuid()),
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName")
					.EqualTo("Account does not exist."));
		}

		[Test]
		public async Task IsAccountDeleted_ShouldReturnTrue_WhenAccountIsDeleted()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountDeleted(this.Account2User1.Id);

			//Assert
			Assert.That(response, Is.True);
		}

		[Test]
		public async Task IsAccountDeleted_ShouldReturnFalse_WhenAccountIsNotDeleted()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountDeleted(this.Account1User1.Id);

			//Assert
			Assert.That(response, Is.False);
		}

		[Test]
		public void IsAccountDeleted_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await accountService.IsAccountDeleted(Guid.NewGuid()),
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName")
					.EqualTo("Account does not exist."));
		}

		[Test]
		public async Task DeleteAccountById_ShouldDeleteAccountWithoutTransactions_WithValidId()
		{
			//Arrange
			Guid accountId = Guid.NewGuid();
			Account account = new Account
			{
				Id = accountId,
				Name = "AccountForDelete",
				AccountTypeId = this.AccountType1.Id,
				Balance = 0,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};
			Guid transactionId = Guid.NewGuid();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				AccountId = accountId,
				Amount = 200,
				CategoryId = this.Category4.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			data.Accounts.Add(account);
			data.Transactions.Add(transaction);
			data.SaveChanges();

			//Assert that the Account and Transactions are created and Account is not deleted
			Assert.That(account.IsDeleted, Is.False);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));

			//Act
			await accountService.DeleteAccountById(accountId, this.User1.Id, false);

			//Assert that the Account is deleted but Transactions not
			Assert.That(account.IsDeleted, Is.True);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task DeleteAccountById_ShouldDeleteAccountAndTransactions_WithValidId()
		{
			//Arrange
			Guid accountId = Guid.NewGuid();
			Account account = new Account
			{
				Id = accountId,
				Name = "AccountForDelete",
				AccountTypeId = this.AccountType1.Id,
				Balance = 0,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};
			Guid transactionId = Guid.NewGuid();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				AccountId = accountId,
				Amount = 200,
				CategoryId = this.Category4.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			data.Accounts.Add(account);
			data.Transactions.Add(transaction);
			data.SaveChanges();

			//Assert that the Account and Transactions are created and Account is not deleted
			Assert.That(account.IsDeleted, Is.False);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));

			//Arrange
			int accountsCountBefore = data.Accounts.Count();
			int transactionsCountBefore = data.Transactions.Count();

			//Act
			await accountService.DeleteAccountById(accountId, this.User1.Id, true);

			//Assert that the Account is deleted but Transactions not
			Assert.That(data.Accounts.Count(), Is.EqualTo(accountsCountBefore - 1));
			Assert.That(data.Transactions.Count(), Is.EqualTo(transactionsCountBefore - 1));
		}

		[Test]
		public void DeleteAccountById_ShouldThrowException_WhenIdIsInvalid()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await accountService.DeleteAccountById(Guid.NewGuid(), this.User1.Id, true),
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName")
					.EqualTo("Account does not exist."));
		}

		//[Test]
		//public async Task DashboardViewModel_ShouldReturnCorrectData_WithValidInput()
		//{
		//	//Arrange
		//	DateTime startDate = DateTime.UtcNow.AddMonths(-1);
		//	DateTime endDate = DateTime.UtcNow;
		//	IEnumerable<AccountCardViewModel> expectedAccounts = data.Accounts
		//		.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
		//		.Select(a => mapper.Map<AccountCardViewModel>(a))
		//		.AsEnumerable();

		//	IEnumerable<TransactionShortViewModel> expectedLastTransactions = data.Transactions
		//		.Where(t =>
		//			t.Account.OwnerId == this.User1.Id &&
		//			t.CreatedOn >= startDate &&
		//			t.CreatedOn <= endDate)
		//		.OrderByDescending(t => t.CreatedOn)
		//		.Take(5)
		//		.Select(t => mapper.Map<TransactionShortViewModel>(t))
		//		.AsEnumerable();

		//	var expectedCashFlow = new Dictionary<string, CashFlowViewModel>();

		//	await data.Accounts
		//		.Where(a => a.OwnerId == this.User1.Id && a.Transactions.Any())
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
		//				expectedCashFlow[a.Currency.Name].Income += (decimal)income;
		//			}

		//			decimal? expense = a.Transactions?
		//				.Where(t => t.TransactionType == TransactionType.Expense)
		//				.Sum(t => t.Amount);

		//			if (expense != null)
		//			{
		//				expectedCashFlow[a.Currency.Name].Expence += (decimal)expense;
		//			}
		//		});

		//	//Act
		//	var actualDashboard = new DashboardServiceModel
		//	{
		//		StartDate = startDate,
		//		EndDate = endDate
		//	};

		//	await accountService.DashboardViewModel(this.User1.Id, actualDashboard);

		//	//Assert
		//	Assert.That(actualDashboard.Accounts.Count(), Is.EqualTo(expectedAccounts.Count()));
		//	for (int i = 0; i < actualDashboard.Accounts.Count(); i++)
		//	{
		//		Assert.That(actualDashboard.Accounts.ElementAt(i).Id,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Id));
		//		Assert.That(actualDashboard.Accounts.ElementAt(i).Name,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Name));
		//	}

		//	Assert.That(actualDashboard.LastTransactions.Count(), 
		//		Is.EqualTo(expectedLastTransactions.Count()));
		//	for (int i = 0; i < actualDashboard.LastTransactions.Count(); i++)
		//	{
		//		Assert.That(actualDashboard.LastTransactions.ElementAt(i).Id,
		//			Is.EqualTo(expectedLastTransactions.ElementAt(i).Id));
		//		Assert.That(actualDashboard.LastTransactions.ElementAt(i).Amount,
		//			Is.EqualTo(expectedLastTransactions.ElementAt(i).Amount));
		//	}

		//	Assert.That(actualDashboard.CurrenciesCashFlow.Count,
		//		Is.EqualTo(expectedCashFlow.Count));

		//	foreach (string expectedKey in expectedCashFlow.Keys)
		//	{
		//		Assert.That(actualDashboard.CurrenciesCashFlow.ContainsKey(expectedKey), Is.True);

		//		Assert.That(actualDashboard.CurrenciesCashFlow[expectedKey].Income, 
		//			Is.EqualTo(expectedCashFlow[expectedKey].Income));

		//		Assert.That(actualDashboard.CurrenciesCashFlow[expectedKey].Expence, 
		//			Is.EqualTo(expectedCashFlow[expectedKey].Expence));
		//	}
		//}

		//[Test]
		//public void DashboardViewModel_ShouldThrowException_WithInvalidDates()
		//{
		//	//Arrange
		//	DashboardServiceModel dashboardModel = new DashboardServiceModel
		//	{
		//		StartDate = DateTime.UtcNow,
		//		EndDate = DateTime.UtcNow.AddMonths(-1)
		//	};

		//	//Act & Assert
		//	Assert.That(async () => await accountService.DashboardViewModel(this.User1.Id, dashboardModel),
		//		Throws.TypeOf<ArgumentException>().With.Message
		//			.EqualTo("Start Date must be before End Date."));
		//}
	}
}
