using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Tests.Services
{
    [TestFixture]
	class AccountServiceTests : UnitTestsBase
	{
		private IAccountsService accountService;

		[SetUp]
		public void SetUp()
		{
			this.accountService = new AccountsService(this.data, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task CreateTransaction_ShouldAddNewTransaction_AndChangeAccountBalance()
		{
			//Arrange
			TransactionFormModel transactionModel = new TransactionFormModel()
			{
				Amount = 100,
				AccountId = this.Account1User1.Id,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Not Initial Balance",
				TransactionType = TransactionType.Expense
			};
			int transactionsCountBefore = this.Account1User1.Transactions.Count();
			decimal balanceBefore = this.Account1User1.Balance;

			//Act
			string id = await accountService.CreateTransaction(this.User1.Id, transactionModel);
			Transaction? transaction = await data.Transactions.FindAsync(id);

			//Assert
			Assert.That(transaction, Is.Not.Null);
			Assert.That(this.Account1User1.Transactions.Count(), Is.EqualTo(transactionsCountBefore + 1));
			Assert.That(transaction.Amount, Is.EqualTo(transactionModel.Amount));
			Assert.That(transaction.CategoryId, Is.EqualTo(transactionModel.CategoryId));
			Assert.That(transaction.AccountId, Is.EqualTo(transactionModel.AccountId));
			Assert.That(transaction.Refference, Is.EqualTo(transactionModel.Refference));
			Assert.That(transaction.CreatedOn, Is.EqualTo(transactionModel.CreatedOn));
			Assert.That(transaction.TransactionType, Is.EqualTo(transactionModel.TransactionType));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transaction.Amount));
		}


		[Test]
		public async Task DeleteTransactionById_ShouldDeleteIncomeTransactionReductBalanceAndNewBalance_WithValidInput()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
				AccountId = this.Account1User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TestTransaction",
				TransactionType = TransactionType.Income
			};
			await data.Transactions.AddAsync(transaction);
			this.Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();

			decimal balanceBefore = this.Account1User1.Balance;
			int transactionsBefore = this.Account1User1.Transactions.Count;
			Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

			//Assert
			Assert.That(transactionInDb, Is.Not.Null);

			//Act
			decimal newBalance = await accountService.DeleteTransaction(transactionId);

			//Assert
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transactionInDb.Amount));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(newBalance));
			Assert.That(this.Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
			Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
		}

		[Test]
		public void DeleteTransactionById_ShouldThrowAnException_WithInvalidInput()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await accountService.DeleteTransaction(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task EditTransactionFormModelById_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionFormModel? transactionFormModel = await accountService
				.GetFulfilledTransactionFormModel(this.Transaction1.Id);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			//Assert.That(transactionFormModel.Id, Is.EqualTo(this.Transaction1.Id));
			Assert.That(transactionFormModel.AccountId, Is.EqualTo(this.Transaction1.AccountId));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1.Amount));
			Assert.That(transactionFormModel.CategoryId, Is.EqualTo(this.Transaction1.CategoryId));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(this.Transaction1.Refference));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1.TransactionType));
		}

		[Test]
		public void EditTransactionFormModelById_ShouldReturnNull_WithInValidInput()
		{
			//Act & Assert
			Assert.That(async () => await accountService.GetFulfilledTransactionFormModel(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task TransactionViewModel_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionDetailsViewModel transactionFormModel =
				await accountService.GetTransactionViewModel(this.Transaction1.Id);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			Assert.That(transactionFormModel.Id, Is.EqualTo(this.Transaction1.Id));
			Assert.That(transactionFormModel.AccountName, Is.EqualTo(this.Transaction1.Account.Name));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1.Amount));
			Assert.That(transactionFormModel.CategoryName, Is.EqualTo(this.Transaction1.Category.Name));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(this.Transaction1.Refference));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1.TransactionType.ToString()));
		}

		[Test]
		public void TransactionViewModel_ShouldReturnNull_WithInValidInput()
		{
			//Act & Assert
			Assert.That(async () => await accountService.GetTransactionViewModel(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task AllTransactionsViewModel_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Arrange
			var model = new UserTransactionsViewModel
			{
				StartDate = DateTime.Now.AddMonths(-1),
				EndDate = DateTime.Now
			};

			IEnumerable<Transaction> expectedTransactions = await data.Transactions
				.Where(t => t.Account.OwnerId == this.User1.Id &&
					t.CreatedOn >= model.StartDate &&
					t.CreatedOn <= model.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.ToListAsync();

			//Act
			await accountService.SetUserTransactionsViewModel(this.User1.Id, model);

			//Assert
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Transactions.Count(), Is.EqualTo(expectedTransactions.Count()));
			for (int i = 0; i < expectedTransactions.Count(); i++)
			{
				Assert.That(model.Transactions.ElementAt(i).Id,
					Is.EqualTo(expectedTransactions.ElementAt(i).Id));
				Assert.That(model.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expectedTransactions.ElementAt(i).Amount));
				Assert.That(model.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(expectedTransactions.ElementAt(i).Category.Name));
				Assert.That(model.Transactions.ElementAt(i).Refference,
					Is.EqualTo(expectedTransactions.ElementAt(i).Refference));
				Assert.That(model.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(expectedTransactions.ElementAt(i).TransactionType.ToString()));
			}
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalance_WhenTransactionTypeIsChanged()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
				AccountId = this.Account1User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TransactionTypeChanged",
				TransactionType = TransactionType.Income
			};

			data.Transactions.Add(transaction);
			this.Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal balanceBefore = this.Account1User1.Balance;

			TransactionFormModel transactionEditModel = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => mapper.Map<TransactionFormModel>(t))
				.FirstAsync();

			//Act
			transactionEditModel.TransactionType = TransactionType.Expense;
			await accountService.EditTransaction(transactionId.ToString(), transactionEditModel);

			//Assert
			Assert.That(transaction.TransactionType, Is.EqualTo(TransactionType.Expense));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transaction.Amount * 2));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalanceOnTwoAccounts_WhenAccountIsChanged()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
				AccountId = this.Account2User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "AccountChanged",
				TransactionType = TransactionType.Income
			};

			data.Transactions.Add(transaction);
			this.Account2User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal firstAccBalanceBefore = this.Account2User1.Balance;
			decimal secondAccBalanceBefore = this.Account1User1.Balance;

			//Act
			TransactionFormModel editTransactionModel = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => mapper.Map<TransactionFormModel>(t))
				.FirstAsync();

			editTransactionModel.AccountId = this.Account1User1.Id;
			await accountService.EditTransaction(transactionId.ToString(), editTransactionModel);

			//Assert
			Assert.That(transaction.AccountId, Is.EqualTo(this.Account1User1.Id));
			Assert.That(this.Account2User1.Balance, Is.EqualTo(firstAccBalanceBefore - transaction.Amount));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(secondAccBalanceBefore + transaction.Amount));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransaction_WhenPaymentRefferenceIsChanged()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
				AccountId = this.Account1User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "First Refference",
				TransactionType = TransactionType.Income
			};

			await data.Transactions.AddAsync(transaction);
			this.Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal balanceBefore = this.Account1User1.Balance;
			string categoryIdBefore = transaction.CategoryId;

			//Act
			TransactionFormModel editTransactionModel = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => mapper.Map<TransactionFormModel>(t))
				.FirstAsync();
			editTransactionModel.Refference = "Second Refference";
			await accountService.EditTransaction(transactionId.ToString(), editTransactionModel);

			//Assert that only transaction refference is changed
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore));
			Assert.That(transaction.Refference, Is.EqualTo(editTransactionModel.Refference));
			Assert.That(transaction.CategoryId, Is.EqualTo(categoryIdBefore));
		}

		//[Test]
		//public async Task DeleteTransactionById_ShouldDeleteExpenseTransactionIncreaseBalanceAndNewBalance_WithValidInput()
		//{
		//	//Arrange
		//	string transactionId = Guid.NewGuid().ToString();
		//	Transaction transaction = new Transaction
		//	{
		//		Id = transactionId,
		//		AccountId = this.Account1User1.Id,
		//		Amount = 123,
		//		CategoryId = this.Category2.Id,
		//		CreatedOn = DateTime.UtcNow,
		//		Refference = "TestTransaction",
		//		TransactionType = TransactionType.Expense
		//	};
		//	data.Transactions.Add(transaction);
		//	this.Account1User1.Balance -= transaction.Amount;
		//	await data.SaveChangesAsync();

		//	decimal balanceBefore = this.Account1User1.Balance;
		//	int transactionsBefore = this.Account1User1.Transactions.Count;
		//	Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

		//	//Assert
		//	Assert.That(transactionInDb, Is.Not.Null);

		//	//Act
		//	decimal newBalance = await transactionService.DeleteTransaction(transactionId);

		//	//Assert
		//	Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
		//	Assert.That(this.Account1User1.Balance, Is.EqualTo(newBalance));
		//	Assert.That(this.Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
		//	Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
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
						.Select(t => new TransactionTableViewModel
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = a.Currency.Name,
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
			var inputModel = new AccountFormModel
			{
				Name = "AccountWithNonZeroBalance",
				AccountTypeId = this.AccountType1.Id,
				Balance = 100,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};

			int accountsCountBefore = data.Accounts.Count();

			//Act
			string newAccountId = await accountService.CreateAccount(inputModel);
			Account newAccount = await data.Accounts
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstAsync();

			//Assert
			Assert.That(data.Accounts.Count(), Is.EqualTo(accountsCountBefore + 1));
			Assert.That(newAccount.Name, Is.EqualTo(inputModel.Name));
			Assert.That(newAccount.Balance, Is.EqualTo(inputModel.Balance));
			Assert.That(newAccount.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(newAccount.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
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
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};

			int accountsCountBefore = data.Accounts.Count();

			//Act
			string newAccountId = await accountService.CreateAccount(newAccountModel);
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
				OwnerId = this.User1.Id,
				AccountId = accountId,
				Amount = 200,
				CategoryId = this.Category4.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			await data.Accounts.AddAsync(account);
			await data.Transactions.AddAsync(transaction);
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
				OwnerId = this.User1.Id,
				AccountId = accountId,
				Amount = 200,
				CategoryId = this.Category4.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			await data.Accounts.AddAsync(account);
			await data.Transactions.AddAsync(transaction);
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
	}
}
