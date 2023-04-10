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
		public void CreateAccount_ShouldThrowException_WhenUserHaveAccountWithSameName()
		{
			//Arrange
			AccountFormModel newAccountModel = new AccountFormModel
			{
				Name = this.Account1User1.Name,
				AccountTypeId = this.AccountType1.Id,
				Balance = 0,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};

			// Act & Assert
			Assert.That(async () => await accountService.CreateAccount(newAccountModel),
			Throws.TypeOf<ArgumentException>().With.Message
				.EqualTo($"The User already have Account with {this.Account1User1.Name} name."));
		}

		[Test]
		public async Task CreateTransaction_ShouldAddNewTransaction_AndDecreaseChangeAccountBalance()
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
		public async Task CreateTransaction_ShouldAddNewTransaction_AndIncreaseChangeAccountBalance()
		{
			//Arrange
			TransactionFormModel transactionModel = new TransactionFormModel()
			{
				Amount = 100,
				AccountId = this.Account1User1.Id,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Not Initial Balance",
				TransactionType = TransactionType.Income
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
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore + transaction.Amount));
		}

		[Test]
		public void CreateTransaction_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange
			var inputFormModel = new TransactionFormModel
			{
				Amount = 100,
				AccountId = Guid.NewGuid().ToString(),
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Not Initial Balance",
				TransactionType = TransactionType.Expense
			};

			//Act & Assert
			Assert.That(async () => await accountService.CreateTransaction(this.User1.Id, inputFormModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Account does not exist."));
				
		}
		
		[Test]
		public async Task DeleteAccount_ShouldDeleteAccountWithoutTransactions_WithValidId()
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
		public async Task DeleteAccount_ShouldDeleteAccountAndTransactions_WithValidId()
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
		public void DeleteAccount_ShouldThrowException_WhenIdIsInvalid()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await accountService.DeleteAccount(Guid.NewGuid().ToString(), true, this.User1.Id),
				Throws.TypeOf<InvalidOperationException>());
		}
		
		[Test]
		public void DeleteAccount_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await accountService.DeleteAccount(this.Account1User1.Id, true, this.User2.Id),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Can't delete someone else account."));
		}

		[Test]
		public async Task DeleteTransaction_ShouldDeleteIncomeTransactionReductBalanceAndNewBalance_WithValidInput()
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
		public async Task DeleteTransaction_ShouldDeleteExpenseTransactionIncreaseBalanceAndNewBalance_WithValidInput()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				AccountId = this.Account1User1.Id,
				OwnerId = this.User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TestTransaction",
				TransactionType = TransactionType.Expense
			};
			await data.Transactions.AddAsync(transaction);
			this.Account1User1.Balance -= transaction.Amount;
			await data.SaveChangesAsync();

			decimal balanceBefore = this.Account1User1.Balance;
			int transactionsBefore = this.Account1User1.Transactions.Count;
			Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

			//Assert
			Assert.That(transactionInDb, Is.Not.Null);

			//Act
			decimal newBalance = await accountService.DeleteTransaction(transactionId);

			//Assert
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(newBalance));
			Assert.That(this.Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
			Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
		}

		[Test]
		public void DeleteTransaction_ShouldThrowAnException_WithInvalidInput()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await accountService.DeleteTransaction(Guid.NewGuid().ToString()),
			Throws.TypeOf<InvalidOperationException>());
		}
		
		[Test]
		public void DeleteTransaction_ShouldThrowAnException_WithUserIsNotOwner()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await accountService.DeleteTransaction(this.Transaction1User1.Id, this.User2.Id),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo("User is now transaction's owner"));
		}
				
		[Test]
		public async Task EditAccount_ShouldChangeAccountName()
		{
			//Arrange
			var inputModel = new AccountFormModel
			{
				Name = "New Name", // Change
				AccountTypeId = this.Account2User1.AccountTypeId,
				Balance = this.Account2User1.Balance,
				CurrencyId = this.Account2User1.CurrencyId,
				OwnerId = this.Account2User1.OwnerId
			};

			//Act
			await accountService.EditAccount(this.Account2User1.Id, inputModel);

			//Assert
			Assert.That(this.Account2User1.Name, Is.EqualTo(inputModel.Name));
			Assert.That(this.Account2User1.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(this.Account2User1.Balance, Is.EqualTo(inputModel.Balance));
			Assert.That(this.Account2User1.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(this.Account2User1.OwnerId, Is.EqualTo(inputModel.OwnerId));
		}
			
		[Test]
		public async Task EditAccount_ShouldChangeAccountAccountType()
		{
			//Arrange
			var inputModel = new AccountFormModel
			{
				Name = this.Account2User1.Name,
				AccountTypeId = this.AccountType1.Id, // Change
				Balance = this.Account2User1.Balance,
				CurrencyId = this.Account2User1.CurrencyId,
				OwnerId = this.Account2User1.OwnerId
			};

			//Act
			await accountService.EditAccount(this.Account2User1.Id, inputModel);

			//Assert
			Assert.That(this.Account2User1.Name, Is.EqualTo(inputModel.Name));
			Assert.That(this.Account2User1.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(this.Account2User1.Balance, Is.EqualTo(inputModel.Balance));
			Assert.That(this.Account2User1.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(this.Account2User1.OwnerId, Is.EqualTo(inputModel.OwnerId));
		}
		
		[Test]
		public async Task EditAccount_ShouldChangeAccountBalanceAndInitialBalanceTransactionAmount()
		{
			//Arrange
			decimal accountExpensesSum = this.Account2User1.Transactions
				.Where(t => t.TransactionType == TransactionType.Expense)
				.Sum(t => t.Amount);

			decimal accountIncomesSum = this.Account2User1.Transactions
				.Where(t => t.TransactionType == TransactionType.Income)
				.Sum(t => t.Amount);

			Transaction initialBalTransaction = this.Account2User1.Transactions
				.First(t => t.IsInitialBalance);
			decimal initBalTransactionAmountBefore = initialBalTransaction.Amount;

			//Assert that the account has correct balance before the test
			Assert.That(this.Account2User1.Balance, Is.EqualTo(accountIncomesSum - accountExpensesSum));

			var inputModel = new AccountFormModel
			{
				Name = this.Account2User1.Name,
				AccountTypeId = this.Account2User1.AccountTypeId,
				Balance = this.Account2User1.Balance + 100, // Change
				CurrencyId = this.Account2User1.CurrencyId,
				OwnerId = this.Account2User1.OwnerId
			};

			//Act
			await accountService.EditAccount(this.Account2User1.Id, inputModel);

			//Assert
			Assert.That(this.Account2User1.Balance, Is.EqualTo(accountIncomesSum - accountExpensesSum + 100));
			Assert.That(initBalTransactionAmountBefore, Is.EqualTo(initialBalTransaction.Amount - 100));
			Assert.That(this.Account2User1.Name, Is.EqualTo(inputModel.Name));
			Assert.That(this.Account2User1.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(this.Account2User1.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(this.Account2User1.OwnerId, Is.EqualTo(inputModel.OwnerId));
		}
		
		[Test]
		public async Task EditAccount_ShouldChangeAccountBalanceAndCreateInitialBalanceTransaction()
		{
			//Arrange
			int transactionsCountBefore = this.Account2User2.Transactions.Count();
			
			//Assert that the account doesn't have any transactions
			Assert.That(transactionsCountBefore, Is.EqualTo(0));

			var inputModel = new AccountFormModel
			{
				Name = this.Account2User2.Name,
				AccountTypeId = this.Account2User2.AccountTypeId,
				Balance = this.Account2User2.Balance + 100, // Change
				CurrencyId = this.Account2User2.CurrencyId,
				OwnerId = this.Account2User2.OwnerId
			};

			//Act
			await accountService.EditAccount(this.Account2User2.Id, inputModel);
			
			//Arrange
			Transaction initialBalTransaction = this.Account2User2.Transactions
				.First(t => t.IsInitialBalance);

			//Assert
			Assert.That(this.Account2User2.Balance, Is.EqualTo(100));
			Assert.That(initialBalTransaction.Amount, Is.EqualTo(100));
			Assert.That(this.Account2User2.Name, Is.EqualTo(inputModel.Name));
			Assert.That(this.Account2User2.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(this.Account2User2.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(this.Account2User2.OwnerId, Is.EqualTo(inputModel.OwnerId));
		}

		[Test]
		public void EditAccount_ShouldThrowExceptionWhenUserHaveAccountWithSameName()
		{
			//Arrange
			var inputModel = new AccountFormModel
			{
				Name = this.Account2User1.Name, // Change
				AccountTypeId = this.Account1User1.AccountTypeId,
				Balance = this.Account1User1.Balance + 100,
				CurrencyId = this.Account1User1.CurrencyId,
				OwnerId = this.Account1User1.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await accountService.EditAccount(this.Account1User1.Id, inputModel),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo($"The User already have Account with {this.Account2User1.Name} name."));
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
		
		[Test]
		public async Task GetAccountDetailsViewModel_ShouldReturnAccountDetails_WithValidId()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = this.Account1User1.Id,
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};
			int page = 1;

			AccountDetailsViewModel? expected = await data.Accounts
				.Where(a => a.Id == this.Account1User1.Id && !a.IsDeleted)
				.Select(a => new AccountDetailsViewModel
				{
					Name = a.Name,
					Balance = a.Balance,
					OwnerId = this.User1.Id,
					CurrencyName = a.Currency.Name,
					StartDate = inputModel.StartDate,
					EndDate = inputModel.EndDate,
					Pagination = new PaginationModel
					{
						Page = page,
						TotalElements = a.Transactions.Count(t => t.CreatedOn >= inputModel.StartDate && t.CreatedOn <= inputModel.EndDate)
					},
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= inputModel.StartDate && t.CreatedOn <= inputModel.EndDate)
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
			AccountDetailsViewModel actual = await accountService.GetAccountDetailsViewModel(inputModel);

			//Assert
			Assert.That(actual.Name, Is.EqualTo(expected.Name));
			Assert.That(actual.Balance, Is.EqualTo(expected.Balance));
			Assert.That(actual.CurrencyName, Is.EqualTo(expected.CurrencyName));
			Assert.That(actual.OwnerId, Is.EqualTo(expected.OwnerId));
			Assert.That(actual.Transactions.Count(), Is.EqualTo(expected.Transactions.Count()));

			for (int i = 0; i < expected.Transactions.Count(); i++)
			{
				Assert.That(actual.Transactions.ElementAt(i).Id,
					Is.EqualTo(expected.Transactions.ElementAt(i).Id));
				Assert.That(actual.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expected.Transactions.ElementAt(i).Amount));
				Assert.That(actual.Transactions.ElementAt(i).Refference,
					Is.EqualTo(expected.Transactions.ElementAt(i).Refference));
				Assert.That(actual.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(expected.Transactions.ElementAt(i).TransactionType));
				Assert.That(actual.Transactions.ElementAt(i).AccountCurrencyName,
					Is.EqualTo(expected.Transactions.ElementAt(i).AccountCurrencyName));
				Assert.That(actual.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(expected.Transactions.ElementAt(i).CategoryName));
				Assert.That(actual.Transactions.ElementAt(i).CreatedOn,
					Is.EqualTo(expected.Transactions.ElementAt(i).CreatedOn));
			}
		}

		[Test]
		public void GetAccountDetailsViewModel_ShouldThrowException_WithInvalidId()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = Guid.NewGuid().ToString(),
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			//Act & Assert
			Assert.That(async () => await accountService.GetAccountDetailsViewModel(inputModel),
				Throws.TypeOf<InvalidOperationException>());
		}
		
		[Test]
		public async Task GetAccountDropdownViewModel_ShouldReturnAccount_WithValidId()
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
		public void GetAccountDropdownViewModel_ShouldThrowException_WithInvalidId()
		{
			//Act & Assert
			Assert.That(async () => await accountService.GetAccountDropdownViewModel(Guid.NewGuid().ToString()),
			Throws.TypeOf<InvalidOperationException>());
		}
		
		[Test]
		public async Task GetDeleteAccountViewModel_ShouldReturnCorrectViewModel_WithValidId()
		{
			//Act
			var actualViewModel = await accountService.GetDeleteAccountViewModel(this.Account1User1.Id);

			//Assert
			Assert.That(actualViewModel, Is.Not.Null);
			//Assert.That(actualViewModel.Id, Is.EqualTo(this.Account1User1.Id));
			Assert.That(actualViewModel.Name, Is.EqualTo(this.Account1User1.Name));
			//Assert.That(actualViewModel.OwnerId, Is.EqualTo(this.Account1User1.OwnerId));
		}

		[Test]
		public async Task GetFulfilledTransactionFormModel_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionFormModel? transactionFormModel = await accountService
				.GetFulfilledTransactionFormModel(this.Transaction1User1.Id);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			//Assert.That(transactionFormModel.Id, Is.EqualTo(this.Transaction1.Id));
			Assert.That(transactionFormModel.AccountId, Is.EqualTo(this.Transaction1User1.AccountId));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1User1.Amount));
			Assert.That(transactionFormModel.CategoryId, Is.EqualTo(this.Transaction1User1.CategoryId));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(this.Transaction1User1.Refference));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1User1.TransactionType));
		}

		[Test]
		public void GetFulfilledTransactionFormModel_ShouldThrowException_WhenTransactionDoesNotExist()
		{
			//Act & Assert
			Assert.That(async () => await accountService.GetFulfilledTransactionFormModel(Guid.NewGuid().ToString()),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionViewModel_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionDetailsViewModel transactionFormModel =
				await accountService.GetTransactionViewModel(this.Transaction1User1.Id);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			Assert.That(transactionFormModel.Id, Is.EqualTo(this.Transaction1User1.Id));
			Assert.That(transactionFormModel.AccountName, Is.EqualTo(this.Transaction1User1.Account.Name));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1User1.Amount));
			Assert.That(transactionFormModel.CategoryName, Is.EqualTo(this.Transaction1User1.Category.Name));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(this.Transaction1User1.Refference));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1User1.TransactionType.ToString()));
		}

		[Test]
		public void GetTransactionViewModel_ShouldReturnNull_WithInValidInput()
		{
			//Act & Assert
			Assert.That(async () => await accountService.GetTransactionViewModel(Guid.NewGuid().ToString()),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetUserTransactionsViewModel_ShouldReturnCorrectViewModel_WithValidInput()
		{
			//Arrange
			var dateFilterModel = new DateFilterModel
			{
				StartDate = DateTime.Now.AddMonths(-1),
				EndDate = DateTime.Now
			};

			IEnumerable<Transaction> expectedTransactions = await data.Transactions
				.Where(t => t.Account.OwnerId == this.User1.Id &&
					t.CreatedOn >= dateFilterModel.StartDate &&
					t.CreatedOn <= dateFilterModel.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.ToListAsync();

			//Act
			UserTransactionsViewModel actual =
				await accountService.GetUserTransactionsViewModel(this.User1.Id, dateFilterModel);

			//Assert
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Transactions.Count(), Is.EqualTo(expectedTransactions.Count()));
			for (int i = 0; i < expectedTransactions.Count(); i++)
			{
				Assert.That(actual.Transactions.ElementAt(i).Id,
					Is.EqualTo(expectedTransactions.ElementAt(i).Id));
				Assert.That(actual.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expectedTransactions.ElementAt(i).Amount));
				Assert.That(actual.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(expectedTransactions.ElementAt(i).Category.Name));
				Assert.That(actual.Transactions.ElementAt(i).Refference,
					Is.EqualTo(expectedTransactions.ElementAt(i).Refference));
				Assert.That(actual.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(expectedTransactions.ElementAt(i).TransactionType.ToString()));
			}
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
	}
}
