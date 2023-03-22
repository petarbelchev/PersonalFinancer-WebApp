using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.Transactions;

namespace PersonalFinancer.Tests.Services
{
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
			this.accountService = new AccountService(this.data, this.mapper, this.transactionsService, this.memoryCache);
		}


		//[Test]
		//public void AccountsCount_ShouldReturnAccountsCount()
		//{
		//	//Arrange
		//	int expectedCount = data.Accounts.Count(a => !a.IsDeleted);

		//	//Act
		//	int actualCount = accountService.GetUsersAccountsCount();

		//	//Assert
		//	Assert.That(actualCount, Is.EqualTo(expectedCount));
		//}

		//[Test]
		//public async Task AllAccountsCardViewModel_ShouldReturnUsersAccounts_WithValidId()
		//{
		//	//Arrange
		//	var expectedAccounts = await data.Accounts
		//		.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
		//		.ProjectTo<AccountCardViewModel>(mapper.ConfigurationProvider)
		//		.ToListAsync();

		//	//Act
		//	var actualAccounts = await accountService.GetUserAccountCardsViewModel(this.User1.Id);

		//	//Assert
		//	Assert.That(actualAccounts, Is.Not.Null);
		//	Assert.That(actualAccounts.Count(), Is.EqualTo(expectedAccounts.Count));
		//	for (int i = 0; i < expectedAccounts.Count; i++)
		//	{
		//		Assert.That(actualAccounts.ElementAt(i).Id,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Id));
		//		Assert.That(actualAccounts.ElementAt(i).Name,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Name));
		//		Assert.That(actualAccounts.ElementAt(i).Balance,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Balance));
		//	}
		//}

		//[Test]
		//public async Task AllAccountsDropdownViewModel_ShouldReturnCorrectData_WithValidUserId()
		//{
		//	//Arrange
		//	IEnumerable<AccountDropdownViewModel> expectedAccounts = data.Accounts
		//		.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
		//		.Select(a => mapper.Map<AccountDropdownViewModel>(a))
		//		.AsEnumerable();

		//	//Act
		//	IEnumerable<AccountDropdownViewModel> actualAccounts = 
		//		await accountService.GetUserAccountsDropdownViewModel(this.User1.Id);

		//	//Assert
		//	Assert.That(actualAccounts, Is.Not.Null);
		//	Assert.That(actualAccounts.Count(), Is.EqualTo(expectedAccounts.Count()));

		//	for (int i = 0; i < actualAccounts.Count(); i++)
		//	{
		//		Assert.That(actualAccounts.ElementAt(i).Id,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Id));
		//		Assert.That(actualAccounts.ElementAt(i).Name,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Name));
		//	}
		//}

		[Test]
		public async Task AccountDropdownViewModel_ShouldReturnAccount_WithValidId()
		{
			//Act
			AccountDropdownViewModel? actualAccount = await accountService
				.GetAccountDropdownViewModel(this.Account1User1.Id);

			//Assert
			Assert.That(actualAccount, Is.Not.Null);
			Assert.That(actualAccount.Id, Is.EqualTo(this.Account1User1.Id));
			Assert.That(actualAccount.Name, Is.EqualTo(this.Account1User1.Name));
		}

		[Test]
		public void AccountDropdownViewModel_ShouldReturnNull_WithInvalidId()
		{
			//Act & Assert
			Assert.That(async () => await accountService.GetAccountDropdownViewModel(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task AccountDetailsViewModel_ShouldReturnAccountDetails_WithValidId()
		{
			//Arrange
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;
			int page = 1;

			DetailsAccountViewModel? expected = await data.Accounts
				.Where(a => a.Id == this.Account1User1.Id && !a.IsDeleted)
				.Select(a => new DetailsAccountViewModel
				{
					Name = a.Name,
					Balance = a.Balance,
					CurrencyName = a.Currency.Name,
					StartDate = startDate,
					EndDate = endDate,
					Pagination = new PaginationModel
					{
						Page = page,
						TotalElements = a.Transactions.Count(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
					},
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.OrderByDescending(t => t.CreatedOn)
						.Skip(page != 1 ? 10 * (page - 1) : 0)
						.Take(10)
						.Select(t => new AccountDetailsTransactionViewModel
						{
							Id = t.Id,
							Amount = t.Amount,
							CurrencyName = a.Currency.Name,
							CreatedOn = t.CreatedOn,
							CategoryName = t.Category.Name,
							TransactionType = t.TransactionType.ToString(),
							Refference = t.Refference
						})
						.AsEnumerable()
				})
				.FirstOrDefaultAsync();

			//Aseert
			Assert.That(expected, Is.Not.Null);

			//Act
			DetailsAccountViewModel? actual = await accountService
				.GetAccountDetailsViewModel(this.Account1User1.Id, startDate, endDate);

			//Assert
			Assert.That(actual, Is.Not.Null);
			//Assert.That(actual.Dates.Id, Is.EqualTo(expected.Dates.Id));
			Assert.That(actual.Name, Is.EqualTo(expected.Name));
			Assert.That(actual.Balance, Is.EqualTo(expected.Balance));
			Assert.That(actual.Transactions.Count(), Is.EqualTo(expected.Transactions.Count()));

			for (int i = 0; i < expected.Transactions.Count(); i++)
			{
				Assert.That(actual.Transactions.ElementAt(i).Id,
					Is.EqualTo(expected.Transactions.ElementAt(i).Id));
				Assert.That(actual.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expected.Transactions.ElementAt(i).Amount));
			}
		}

		[Test]
		public void AccountDetailsViewModel_ShouldReturnNull_WithInvalidId()
		{
			//Arrange
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			//Act & Assert
			Assert.That(async () => await accountService.GetAccountDetailsViewModel(Guid.NewGuid().ToString(), startDate, endDate),
				Throws.TypeOf<InvalidOperationException>());
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
			string newAccountId = await accountService.CreateAccount(this.User1.Id, newAccountModel);
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
			string newAccountId = await accountService.CreateAccount(this.User1.Id, newAccountModel);
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
			//Assert
			Assert.That(async () => await accountService.IsAccountOwner(this.User1.Id, Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
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
			Assert.That(async () => await accountService.IsAccountDeleted(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteAccountById_ShouldDeleteAccountWithoutTransactions_WithValidId()
		{
			//Arrange
			string accountId = Guid.NewGuid().ToString();
			Account account = new Account
			{
				Id = accountId,
				Name = "AccountForDelete",
				AccountTypeId = this.AccountType1.Id,
				Balance = 0,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};
			string transactionId = Guid.NewGuid().ToString();
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
			await accountService.DeleteAccount(accountId, false, this.User1.Id);

			//Assert that the Account is deleted but Transactions not
			Assert.That(account.IsDeleted, Is.True);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task DeleteAccountById_ShouldDeleteAccountAndTransactions_WithValidId()
		{
			//Arrange
			string accountId = Guid.NewGuid().ToString();
			Account account = new Account
			{
				Id = accountId,
				Name = "AccountForDelete",
				AccountTypeId = this.AccountType1.Id,
				Balance = 0,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};
			string transactionId = Guid.NewGuid().ToString();
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
			await accountService.DeleteAccount(accountId, true, this.User1.Id);

			//Assert that the Account is deleted but Transactions not
			Assert.That(data.Accounts.Count(), Is.EqualTo(accountsCountBefore - 1));
			Assert.That(data.Transactions.Count(), Is.EqualTo(transactionsCountBefore - 1));
		}

		[Test]
		public void DeleteAccountById_ShouldThrowException_WhenIdIsInvalid()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await accountService.DeleteAccount(Guid.NewGuid().ToString(), true, this.User1.Id),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteAccountViewModel_ShouldReturnCorrectViewModel_WithValidId()
		{
			//Act
			var actualViewModel = await accountService.GetDeleteAccountViewModel(this.Account1User1.Id);

			//Assert
			Assert.That(actualViewModel, Is.Not.Null);
			//Assert.That(actualViewModel.Id, Is.EqualTo(this.Account1User1.Id));
			Assert.That(actualViewModel.Name, Is.EqualTo(this.Account1User1.Name));
			//Assert.That(actualViewModel.OwnerId, Is.EqualTo(this.Account1User1.OwnerId));
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

		[Test]
		public async Task GetAllAccountsCashFlow_ShouldReturnCorrectData()
		{
			//Arrange
			var expected = new Dictionary<string, CashFlowViewModel>();

			await data.Accounts
				.Where(a => a.Transactions.Any())
				.Include(a => a.Currency)
				.Include(a => a.Transactions)
				.ForEachAsync(a =>
				{
					if (!expected.ContainsKey(a.Currency.Name))
					{
						expected[a.Currency.Name] = new CashFlowViewModel();
					}

					decimal? income = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount);

					if (income != null)
					{
						expected[a.Currency.Name].Incomes += (decimal)income;
					}

					decimal? expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						expected[a.Currency.Name].Expenses += (decimal)expense;
					}
				});

			//Act
			var actual = await accountService.GetAllAccountsCashFlow();

			//Assert
			Assert.That(actual.Count, Is.EqualTo(expected.Count));
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.That(actual.ElementAt(i).Key,
					Is.EqualTo(expected.ElementAt(i).Key));
				Assert.That(actual.ElementAt(i).Value.Incomes,
					Is.EqualTo(expected.ElementAt(i).Value.Incomes));
				Assert.That(actual.ElementAt(i).Value.Expenses,
					Is.EqualTo(expected.ElementAt(i).Value.Expenses));
			}
		}
	}
}
