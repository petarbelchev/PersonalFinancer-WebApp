using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using static PersonalFinancer.Data.Constants.PaginationConstants;
using static PersonalFinancer.Data.Constants.CategoryConstants;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	class AccountServiceTests : UnitTestsBase
	{
		private IAccountsService accountService;

		[SetUp]
		public void SetUp()
		{
			accountService = new AccountsService(data, mapper, memoryCache);
		}

		[Test]
		public async Task CreateAccount_ShouldAddNewAccountAndTransaction_WithValidInput()
		{
			//Arrange
			var inputModel = new AccountFormShortServiceModel
			{
				Name = "AccountWithNonZeroBalance",
				AccountTypeId = AccType1User1.Id,
				Balance = 100,
				CurrencyId = Curr1User1.Id,
				OwnerId = User1.Id
			};

			int accountsCountBefore = await data.Accounts.CountAsync();

			//Act
			string newAccountId = await accountService.CreateAccount(inputModel);
			Account newAccount = await data.Accounts
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstAsync();

			//Assert
			Assert.That(await data.Accounts.CountAsync(), Is.EqualTo(accountsCountBefore + 1));
			Assert.That(newAccount.Name, Is.EqualTo(inputModel.Name));
			Assert.That(newAccount.Balance, Is.EqualTo(inputModel.Balance));
			Assert.That(newAccount.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(newAccount.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(newAccount.Transactions.Count, Is.EqualTo(1));
			Assert.That(newAccount.Transactions.First().CategoryId, Is.EqualTo(Cat1User1.Id));
		}

		[Test]
		public async Task CreateAccount_ShouldAddNewAccountWithoutTransaction_WithValidInput()
		{
			//Arrange
			var newAccountModel = new AccountFormShortServiceModel
			{
				Name = "AccountWithZeroBalance",
				AccountTypeId = AccType1User1.Id,
				Balance = 0,
				CurrencyId = Curr1User1.Id,
				OwnerId = User1.Id
			};

			int accountsCountBefore = await data.Accounts.CountAsync();

			//Act
			string newAccountId = await accountService.CreateAccount(newAccountModel);
			Account newAccount = await data.Accounts
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstAsync();

			//Assert
			Assert.That(await data.Accounts.CountAsync(), Is.EqualTo(accountsCountBefore + 1));
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
			var newAccountModel = new AccountFormShortServiceModel
			{
				Name = Account1User1.Name,
				AccountTypeId = AccType1User1.Id,
				Balance = 0,
				CurrencyId = Curr1User1.Id,
				OwnerId = User1.Id
			};

			// Act & Assert
			Assert.That(async () => await accountService.CreateAccount(newAccountModel),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(
				$"The User already have Account with \"{Account1User1.Name}\" name."));
		}

		[Test]
		public async Task CreateTransaction_ShouldAddNewTransaction_AndDecreaseAccountBalance()
		{
			//Arrange
			var transactionModel = new TransactionFormShortServiceModel()
			{
				Amount = 100,
				AccountId = Account1User1.Id,
				OwnerId = User1.Id,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Not Initial Balance",
				TransactionType = TransactionType.Expense
			};
			int transactionsCountBefore = Account1User1.Transactions.Count();
			decimal balanceBefore = Account1User1.Balance;

			//Act
			string id = await accountService.CreateTransaction(transactionModel);
			Transaction? transaction = await data.Transactions.FindAsync(id);

			//Assert
			Assert.That(transaction, Is.Not.Null);
			Assert.That(Account1User1.Transactions.Count(), Is.EqualTo(transactionsCountBefore + 1));
			Assert.That(transaction.Amount, Is.EqualTo(transactionModel.Amount));
			Assert.That(transaction.CategoryId, Is.EqualTo(transactionModel.CategoryId));
			Assert.That(transaction.AccountId, Is.EqualTo(transactionModel.AccountId));
			Assert.That(transaction.Refference, Is.EqualTo(transactionModel.Refference));
			Assert.That(transaction.CreatedOn, Is.EqualTo(transactionModel.CreatedOn));
			Assert.That(transaction.TransactionType, Is.EqualTo(transactionModel.TransactionType));
			Assert.That(Account1User1.Balance, Is.EqualTo(balanceBefore - transaction.Amount));
		}

		[Test]
		public async Task CreateTransaction_ShouldAddNewTransaction_AndIncreaseAccountBalance()
		{
			//Arrange
			var transactionModel = new TransactionFormShortServiceModel()
			{
				Amount = 100,
				AccountId = Account1User1.Id,
				OwnerId = User1.Id,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Not Initial Balance",
				TransactionType = TransactionType.Income
			};
			int transactionsCountBefore = Account1User1.Transactions.Count();
			decimal balanceBefore = Account1User1.Balance;

			//Act
			string id = await accountService.CreateTransaction(transactionModel);
			Transaction? transaction = await data.Transactions.FindAsync(id);

			//Assert
			Assert.That(transaction, Is.Not.Null);
			Assert.That(Account1User1.Transactions.Count(), Is.EqualTo(transactionsCountBefore + 1));
			Assert.That(transaction.Amount, Is.EqualTo(transactionModel.Amount));
			Assert.That(transaction.CategoryId, Is.EqualTo(transactionModel.CategoryId));
			Assert.That(transaction.AccountId, Is.EqualTo(transactionModel.AccountId));
			Assert.That(transaction.Refference, Is.EqualTo(transactionModel.Refference));
			Assert.That(transaction.CreatedOn, Is.EqualTo(transactionModel.CreatedOn));
			Assert.That(transaction.TransactionType, Is.EqualTo(transactionModel.TransactionType));
			Assert.That(Account1User1.Balance, Is.EqualTo(balanceBefore + transaction.Amount));
		}

		[Test]
		public void CreateTransaction_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange
			var inputFormModel = new TransactionFormShortServiceModel
			{
				Amount = 100,
				AccountId = Guid.NewGuid().ToString(),
				OwnerId = User1.Id,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Not Initial Balance",
				TransactionType = TransactionType.Expense
			};

			//Act & Assert
			Assert.That(async () => await accountService.CreateTransaction(inputFormModel),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteAccount_ShouldDeleteAccountWithoutTransactions_WithValidId()
		{
			//Arrange
			string accId = Guid.NewGuid().ToString();
			await data.Accounts.AddAsync(new Account
			{
				Id = accId,
				Name = "AccountForDelete",
				AccountTypeId = AccType1User1.Id,
				Balance = 0,
				CurrencyId = Curr1User1.Id,
				OwnerId = User1.Id,
				Transactions = new HashSet<Transaction>
				{
					new Transaction
					{
						Id = Guid.NewGuid().ToString(),
						OwnerId = User1.Id,
						Amount = 200,
						CategoryId = Cat1User1.Id,
						CreatedOn = DateTime.UtcNow,
						Refference = "Salary",
						TransactionType = TransactionType.Income
					}
				}
			});
			await data.SaveChangesAsync();

			//Assert that the Account and Transactions are created and Account is not deleted
			var account = await data.Accounts.FindAsync(accId);
			Assert.That(account, Is.Not.Null);
			Assert.That(account.IsDeleted, Is.False);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));

			//Act
			await accountService.DeleteAccount(accId, User1.Id, isUserAdmin: false, shouldDeleteTransactions: false);

			//Assert that the Account is mark as deleted but Transactions not
			Assert.That(account.IsDeleted, Is.True);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task DeleteAccount_ShouldDeleteAccountAndTransactions_WithValidId()
		{
			//Arrange
			string accountId = Guid.NewGuid().ToString();
			await data.Accounts.AddAsync(new Account
			{
				Id = accountId,
				Name = "AccountForDelete",
				AccountTypeId = AccType1User1.Id,
				Balance = 0,
				CurrencyId = Curr1User1.Id,
				OwnerId = User1.Id,
				Transactions = new HashSet<Transaction>
				{
					new Transaction
					{
						Id = Guid.NewGuid().ToString(),
						OwnerId = User1.Id,
						Amount = 200,
						CategoryId = Cat1User1.Id,
						CreatedOn = DateTime.UtcNow,
						Refference = "Salary",
						TransactionType = TransactionType.Income
					}
				}
			});
			await data.SaveChangesAsync();

			//Assert that the Account and Transactions are created and Account is not deleted
			var account = await data.Accounts.FindAsync(accountId);
			Assert.That(account, Is.Not.Null);
			Assert.That(account.IsDeleted, Is.False);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));

			//Arrange
			int accountsCountBefore = await data.Accounts.CountAsync();
			int transactionsCountBefore = await data.Transactions.CountAsync();

			//Act
			await accountService.DeleteAccount(accountId, User1.Id, isUserAdmin: false, shouldDeleteTransactions: true);

			//Assert that the Account is deleted but Transactions not
			Assert.That(await data.Accounts.CountAsync(), Is.EqualTo(accountsCountBefore - 1));
			Assert.That(await data.Transactions.CountAsync(), Is.EqualTo(transactionsCountBefore - 1));
		}

		[Test]
		public void DeleteAccount_ShouldThrowException_WhenIdIsInvalid()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await accountService.DeleteAccount(
				Guid.NewGuid().ToString(), User1.Id, isUserAdmin: false, shouldDeleteTransactions: true),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteAccount_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange
			string accId = Guid.NewGuid().ToString();
			await data.Accounts.AddAsync(new Account
			{
				Id = accId,
				AccountTypeId = AccType1User1.Id,
				CurrencyId = Curr1User1.Id,
				Balance = 0,
				Name = "For Delete",
				OwnerId = User1.Id
			});
			await data.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await accountService.DeleteAccount(
				accId, User2.Id, isUserAdmin: false, shouldDeleteTransactions: true),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Can't delete someone else account."));
		}

		[Test]
		public async Task DeleteAccount_ShouldDeleteAccountAndTransactions_WhenUserIsAdmin()
		{
			//Arrange
			string accId = Guid.NewGuid().ToString();
			await data.Accounts.AddAsync(new Account
			{
				Id = accId,
				AccountTypeId = AccType1User1.Id,
				CurrencyId = Curr1User1.Id,
				Balance = 0,
				Name = "For Delete 2",
				OwnerId = User1.Id,
				Transactions = new HashSet<Transaction>
				{
					new Transaction
					{
						Id = Guid.NewGuid().ToString(),
						OwnerId = User1.Id,
						Amount = 200,
						CategoryId = Cat1User1.Id,
						CreatedOn = DateTime.UtcNow,
						Refference = "Salary",
						TransactionType = TransactionType.Income
					}
				}
			});
			await data.SaveChangesAsync();

			//Assert that the Account and Transactions are created and Account is not deleted
			var account = await data.Accounts.FindAsync(accId);
			Assert.That(account, Is.Not.Null);
			Assert.That(account.IsDeleted, Is.False);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));

			//Arrange
			int accountsCountBefore = await data.Accounts.CountAsync();
			int transactionsCountBefore = await data.Transactions.CountAsync();

			//Act
			await accountService.DeleteAccount(accId, User2.Id, isUserAdmin: true, shouldDeleteTransactions: true);

			//Assert that the Account is deleted but Transactions not
			Assert.That(await data.Accounts.CountAsync(), Is.EqualTo(accountsCountBefore - 1));
			Assert.That(await data.Transactions.CountAsync(), Is.EqualTo(transactionsCountBefore - 1));
		}

		[Test]
		public async Task DeleteAccount_ShouldDeleteAccountWithoutTransactions_WhenUserIsAdmin()
		{
			//Arrange
			string accId = Guid.NewGuid().ToString();
			await data.Accounts.AddAsync(new Account
			{
				Id = accId,
				AccountTypeId = AccType1User1.Id,
				CurrencyId = Curr1User1.Id,
				Balance = 0,
				Name = "For Delete 3",
				OwnerId = User1.Id,
				Transactions = new HashSet<Transaction>
				{
					new Transaction
					{
						Id = Guid.NewGuid().ToString(),
						OwnerId = User1.Id,
						Amount = 200,
						CategoryId = Cat1User1.Id,
						CreatedOn = DateTime.UtcNow,
						Refference = "Salary",
						TransactionType = TransactionType.Income
					}
				}
			});
			await data.SaveChangesAsync();

			//Assert that the Account and Transactions are created and Account is not deleted
			var account = await data.Accounts.FindAsync(accId);
			Assert.That(account, Is.Not.Null);
			Assert.That(account.IsDeleted, Is.False);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));

			//Arrange
			int accountsCountBefore = await data.Accounts.CountAsync();
			int transactionsCountBefore = await data.Transactions.CountAsync();

			//Act
			await accountService.DeleteAccount(accId, User2.Id, isUserAdmin: true, shouldDeleteTransactions: false);

			//Assert that the Account is deleted but Transactions not
			Assert.That(account.IsDeleted, Is.True);
			Assert.That(await data.Accounts.CountAsync(), Is.EqualTo(accountsCountBefore));
			Assert.That(await data.Transactions.CountAsync(), Is.EqualTo(transactionsCountBefore));
		}

		[Test]
		public async Task DeleteTransaction_ShouldDeleteIncomeTransactionAndDecreaseBalance_WithValidInput()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			var transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = User1.Id,
				AccountId = Account1User1.Id,
				Amount = 123,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TestTransaction",
				TransactionType = TransactionType.Income
			};
			await data.Transactions.AddAsync(transaction);
			Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();

			decimal balanceBefore = Account1User1.Balance;
			int transactionsBefore = Account1User1.Transactions.Count;
			Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

			//Assert
			Assert.That(transactionInDb, Is.Not.Null);

			//Act
			decimal newBalance = await accountService.DeleteTransaction(transactionId, User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(Account1User1.Balance, Is.EqualTo(balanceBefore - transactionInDb.Amount));
			Assert.That(Account1User1.Balance, Is.EqualTo(newBalance));
			Assert.That(Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
			Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
		}

		[Test]
		public async Task DeleteTransaction_ShouldDeleteIncomeTransactionAndDecreaseBalance_WhenUserIsAdmin()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			var transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = User1.Id,
				AccountId = Account1User1.Id,
				Amount = 123,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TestTransaction",
				TransactionType = TransactionType.Income
			};
			await data.Transactions.AddAsync(transaction);
			Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();

			decimal balanceBefore = Account1User1.Balance;
			int transactionsBefore = Account1User1.Transactions.Count;
			Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

			//Assert
			Assert.That(transactionInDb, Is.Not.Null);

			//Act
			decimal newBalance = await accountService.DeleteTransaction(transactionId, User2.Id, isUserAdmin: true);

			//Assert
			Assert.That(Account1User1.Balance, Is.EqualTo(balanceBefore - transactionInDb.Amount));
			Assert.That(Account1User1.Balance, Is.EqualTo(newBalance));
			Assert.That(Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
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
				AccountId = Account1User1.Id,
				OwnerId = User1.Id,
				Amount = 123,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TestTransaction",
				TransactionType = TransactionType.Expense
			};
			await data.Transactions.AddAsync(transaction);
			Account1User1.Balance -= transaction.Amount;
			await data.SaveChangesAsync();

			decimal balanceBefore = Account1User1.Balance;
			int transactionsBefore = Account1User1.Transactions.Count;
			Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

			//Assert
			Assert.That(transactionInDb, Is.Not.Null);

			//Act
			decimal newBalance = await accountService.DeleteTransaction(transactionId, User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(Account1User1.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
			Assert.That(Account1User1.Balance, Is.EqualTo(newBalance));
			Assert.That(Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
			Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
		}

		[Test]
		public async Task DeleteTransaction_ShouldDeleteExpenseTransactionIncreaseBalanceAndNewBalance_WhenUserIsAdmin()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				AccountId = Account1User1.Id,
				OwnerId = User1.Id,
				Amount = 123,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TestTransaction",
				TransactionType = TransactionType.Expense
			};
			await data.Transactions.AddAsync(transaction);
			Account1User1.Balance -= transaction.Amount;
			await data.SaveChangesAsync();

			decimal balanceBefore = Account1User1.Balance;
			int transactionsBefore = Account1User1.Transactions.Count;
			Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

			//Assert
			Assert.That(transactionInDb, Is.Not.Null);

			//Act
			decimal newBalance = await accountService.DeleteTransaction(transactionId, User2.Id, isUserAdmin: true);

			//Assert
			Assert.That(Account1User1.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
			Assert.That(Account1User1.Balance, Is.EqualTo(newBalance));
			Assert.That(Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
			Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
		}

		[Test]
		public void DeleteTransaction_ShouldThrowAnException_WithInvalidInput()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await accountService.DeleteTransaction(Guid.NewGuid().ToString(), User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteTransaction_ShouldThrowAnException_WithUserIsNotOwner()
		{
			//Arrange
			string id = Guid.NewGuid().ToString();
			var transaction = new Transaction
			{
				Id = id,
				AccountId = Account1User1.Id,
				CategoryId = Curr1User1.Id,
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				OwnerId = User1.Id,
				Refference = "For Delete",
				TransactionType = TransactionType.Expense
			};
			await data.Transactions.AddAsync(transaction);
			await data.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await accountService.DeleteTransaction(id, User2.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo("User is not transaction's owner"));
		}

		[Test]
		public async Task EditAccount_ShouldChangeAccountName()
		{
			//Arrange
			string accId = Guid.NewGuid().ToString();
			var account = new Account
			{
				Id = accId,
				Name = "For Edit",
				Balance = 10,
				AccountTypeId = AccType1User1.Id,
				CurrencyId = Curr1User1.Id,
				OwnerId = User1.Id
			};
			await data.Accounts.AddAsync(account);
			await data.SaveChangesAsync();

			var inputModel = new AccountFormShortServiceModel
			{
				Name = "New Name", // Change
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance,
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act
			await accountService.EditAccount(accId, inputModel);

			//Assert
			Assert.That(account.Name, Is.EqualTo(inputModel.Name));
			Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(account.Balance, Is.EqualTo(inputModel.Balance));
			Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
		}

		[Test]
		public async Task EditAccount_ShouldChangeAccountAccountType()
		{
			//Arrange
			string accId = Guid.NewGuid().ToString();
			var account = new Account
			{
				Id = accId,
				Name = "For Edit2",
				Balance = 10,
				AccountTypeId = AccType1User1.Id,
				CurrencyId = Curr1User1.Id,
				OwnerId = User2.Id
			};
			await data.Accounts.AddAsync(account);
			await data.SaveChangesAsync();

			var inputModel = new AccountFormShortServiceModel
			{
				Name = account.Name,
				AccountTypeId = AccType2User1.Id, // Change
				Balance = account.Balance,
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act
			await accountService.EditAccount(accId, inputModel);

			//Assert
			Assert.That(account.Name, Is.EqualTo(inputModel.Name));
			Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(account.Balance, Is.EqualTo(inputModel.Balance));
			Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
		}

		[Test]
		public async Task EditAccount_ShouldChangeAccountBalanceAndInitialBalanceTransactionAmount()
		{
			//Arrange
			var account = new Account
			{
				Id = Guid.NewGuid().ToString(),
				Name = "For Edit 3",
				Balance = 5,
				AccountTypeId = AccType1User1.Id,
				CurrencyId = Curr1User1.Id,
				OwnerId = User1.Id
			};
			var initialBalTransaction = new Transaction
			{
				Id = Guid.NewGuid().ToString(),
				Amount = 10,
				OwnerId = account.OwnerId,
				AccountId = account.Id,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Initial Balance",
				IsInitialBalance = true,
				TransactionType = TransactionType.Income
			};
			var expenseTransaction = new Transaction
			{
				Id = Guid.NewGuid().ToString(),
				Amount = 5,
				OwnerId = account.OwnerId,
				AccountId = account.Id,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Lunch",
				TransactionType = TransactionType.Expense
			};
			account.Transactions.Add(initialBalTransaction);
			account.Transactions.Add(expenseTransaction);
			await data.Accounts.AddAsync(account);
			await data.SaveChangesAsync();

			decimal initBalTransactionAmountBefore = initialBalTransaction.Amount;

			decimal accountExpensesSum = account.Transactions
				.Where(t => t.TransactionType == TransactionType.Expense)
				.Sum(t => t.Amount);

			decimal accountIncomesSum = account.Transactions
				.Where(t => t.TransactionType == TransactionType.Income)
				.Sum(t => t.Amount);


			//Assert that the account has correct balance before the test
			Assert.That(account.Balance, Is.EqualTo(accountIncomesSum - accountExpensesSum));

			var inputModel = new AccountFormShortServiceModel
			{
				Name = account.Name,
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance + 100, // Change
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act
			await accountService.EditAccount(account.Id, inputModel);

			//Assert
			Assert.That(account.Balance, Is.EqualTo(accountIncomesSum - accountExpensesSum + 100));
			Assert.That(initBalTransactionAmountBefore, Is.EqualTo(initialBalTransaction.Amount - 100));
			Assert.That(account.Name, Is.EqualTo(inputModel.Name));
			Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
		}

		[Test]
		public async Task EditAccount_ShouldChangeAccountBalanceAndCreateInitialBalanceTransaction()
		{
			//Arrange
			var account = new Account
			{
				Id = Guid.NewGuid().ToString(),
				Name = "For Edit 3",
				Balance = 0,
				AccountTypeId = AccType1User1.Id,
				CurrencyId = Curr1User1.Id,
				OwnerId = User1.Id
			};
			await data.Accounts.AddAsync(account);
			await data.SaveChangesAsync();

			var inputModel = new AccountFormShortServiceModel
			{
				Name = account.Name,
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance + 100, // Change
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act
			await accountService.EditAccount(account.Id, inputModel);

			Transaction initialBalTransaction =
				account.Transactions.First(t => t.IsInitialBalance);

			//Assert
			Assert.That(account.Balance, Is.EqualTo(100));
			Assert.That(initialBalTransaction.Amount, Is.EqualTo(100));
			Assert.That(account.Name, Is.EqualTo(inputModel.Name));
			Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
		}

		[Test]
		public void EditAccount_ShouldThrowExceptionWhenUserHaveAccountWithSameName()
		{
			//Arrange
			var inputModel = new AccountFormShortServiceModel
			{
				Name = Account2User1.Name, // Change
				AccountTypeId = Account1User1.AccountTypeId,
				Balance = Account1User1.Balance + 100,
				CurrencyId = Account1User1.CurrencyId,
				OwnerId = Account1User1.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await accountService.EditAccount(Account1User1.Id, inputModel),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(
				$"The User already have Account with \"{Account2User1.Name}\" name."));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalance_WhenTransactionTypeIsChanged()
		{
			//Arrange
			var transaction = new Transaction
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = User1.Id,
				AccountId = Account1User1.Id,
				Amount = 123,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TransactionTypeChanged",
				TransactionType = TransactionType.Income
			};

			await data.Transactions.AddAsync(transaction);
			Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal balanceBefore = Account1User1.Balance;

			TransactionFormShortServiceModel transactionEditModel = await data.Transactions
				.Where(t => t.Id == transaction.Id)
				.Select(t => mapper.Map<TransactionFormShortServiceModel>(t))
				.FirstAsync();

			//Act
			transactionEditModel.TransactionType = TransactionType.Expense;
			await accountService.EditTransaction(transaction.Id, transactionEditModel);

			//Assert
			Assert.That(transaction.TransactionType, Is.EqualTo(transactionEditModel.TransactionType));
			Assert.That(Account1User1.Balance, Is.EqualTo(balanceBefore - transaction.Amount * 2));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalanceOnTwoAccounts_WhenAccountIsChanged()
		{
			//Arrange
			var transaction = new Transaction
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = User1.Id,
				AccountId = Account2User1.Id,
				Amount = 123,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "AccountChanged",
				TransactionType = TransactionType.Income
			};

			await data.Transactions.AddAsync(transaction);
			Account2User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal firstAccBalanceBefore = Account2User1.Balance;
			decimal secondAccBalanceBefore = Account1User1.Balance;

			//Act
			TransactionFormShortServiceModel editTransactionModel = await data.Transactions
				.Where(t => t.Id == transaction.Id)
				.Select(t => mapper.Map<TransactionFormShortServiceModel>(t))
				.FirstAsync();

			editTransactionModel.AccountId = Account1User1.Id;
			await accountService.EditTransaction(transaction.Id, editTransactionModel);

			//Assert
			Assert.That(transaction.AccountId, Is.EqualTo(Account1User1.Id));
			Assert.That(Account2User1.Balance, Is.EqualTo(firstAccBalanceBefore - transaction.Amount));
			Assert.That(Account1User1.Balance, Is.EqualTo(secondAccBalanceBefore + transaction.Amount));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransaction_WhenPaymentRefferenceIsChanged()
		{
			//Arrange
			Transaction transaction = new Transaction
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = User1.Id,
				AccountId = Account1User1.Id,
				Amount = 123,
				CategoryId = Cat1User1.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "First Refference",
				TransactionType = TransactionType.Income
			};

			await data.Transactions.AddAsync(transaction);
			Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal balanceBefore = Account1User1.Balance;

			//Act
			TransactionFormShortServiceModel editTransactionModel = await data.Transactions
				.Where(t => t.Id == transaction.Id)
				.Select(t => mapper.Map<TransactionFormShortServiceModel>(t))
				.FirstAsync();
			editTransactionModel.Refference = "Second Refference";
			await accountService.EditTransaction(transaction.Id, editTransactionModel);

			//Assert that only transaction refference is changed
			Assert.That(Account1User1.Balance, Is.EqualTo(balanceBefore));
			Assert.That(transaction.Refference, Is.EqualTo(editTransactionModel.Refference));
			Assert.That(transaction.CategoryId, Is.EqualTo(editTransactionModel.CategoryId));
			Assert.That(transaction.AccountId, Is.EqualTo(editTransactionModel.AccountId));
			Assert.That(transaction.Amount, Is.EqualTo(editTransactionModel.Amount));
			Assert.That(transaction.OwnerId, Is.EqualTo(editTransactionModel.OwnerId));
			Assert.That(transaction.CreatedOn, Is.EqualTo(editTransactionModel.CreatedOn));
		}

		[Test]
		public async Task GetAccountDetails_ShouldReturnAccountDetails_WithValidId()
		{
			//Arrange
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			AccountDetailsServiceModel? expected = await data.Accounts
				.Where(a => a.Id == Account1User1.Id && !a.IsDeleted)
				.Select(a => new AccountDetailsServiceModel
				{
					Id = a.Id,
					Name = a.Name,
					Balance = a.Balance,
					OwnerId = User1.Id,
					CurrencyName = a.Currency.Name,
					StartDate = startDate,
					EndDate = endDate,
					TotalAccountTransactions = a.Transactions.Count(t =>
						t.CreatedOn >= startDate && t.CreatedOn <= endDate),
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.OrderByDescending(t => t.CreatedOn)
						.Take(TransactionsPerPage)
						.Select(t => new TransactionTableServiceModel
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = a.Currency.Name,
							CreatedOn = t.CreatedOn,
							CategoryName = t.Category.Name,
							TransactionType = t.TransactionType.ToString(),
							Refference = t.Refference
						})
				})
				.FirstOrDefaultAsync();

			//Aseert
			Assert.That(expected, Is.Not.Null);

			//Act
			AccountDetailsServiceModel actual = await accountService.GetAccountDetails(
				Account1User1.Id, startDate, endDate, User1.Id, isUserAdmin: false);

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
		public void GetAccountDetails_ShouldThrowException_WithInvalidId()
		{
			//Arrange
			string id = Guid.NewGuid().ToString();
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			//Act & Assert
			Assert.That(async () => await accountService.GetAccountDetails(id, startDate, endDate, User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountName_ShouldReturnAccountName_WithValidId()
		{
			//Act
			string actualName = await accountService
				.GetAccountName(Account1User1.Id, User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(actualName, Is.EqualTo(Account1User1.Name));
		}

		[Test]
		public void GetAccountName_ShouldThrowException_WithInvalidId()
		{
			//Arrange
			string invalidId = Guid.NewGuid().ToString();

			//Act & Assert
			Assert.That(async () => await accountService.GetAccountName(invalidId, User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountFormData_ShouldReturnCorrectData()
		{
			//Arrange

			//Act
			var formData = await accountService.GetAccountFormData(Account1User1.Id, User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(formData.Name, Is.EqualTo(Account1User1.Name));
			Assert.That(formData.Balance, Is.EqualTo(Account1User1.Balance));
			Assert.That(formData.AccountTypeId, Is.EqualTo(Account1User1.AccountTypeId));
			Assert.That(formData.CurrencyId, Is.EqualTo(Account1User1.CurrencyId));
			Assert.That(formData.OwnerId, Is.EqualTo(Account1User1.OwnerId));
		}

		[Test]
		public async Task GetAccountFormData_ShouldReturnCorrectData_WhenUserIsAdmin()
		{
			//Arrange

			//Act
			var formData = await accountService.GetAccountFormData(Account1User1.Id, User2.Id, isUserAdmin: true);

			//Assert
			Assert.That(formData.Name, Is.EqualTo(Account1User1.Name));
			Assert.That(formData.Balance, Is.EqualTo(Account1User1.Balance));
			Assert.That(formData.AccountTypeId, Is.EqualTo(Account1User1.AccountTypeId));
			Assert.That(formData.CurrencyId, Is.EqualTo(Account1User1.CurrencyId));
			Assert.That(formData.OwnerId, Is.EqualTo(Account1User1.OwnerId));
		}

		[Test]
		public void GetAccountFormData_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange
			string invalidId = Guid.NewGuid().ToString();

			//Act & Assert
			Assert.That(async () => await accountService.GetAccountFormData(invalidId, User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void GetAccountFormData_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await accountService.GetAccountFormData(Account1User1.Id, User2.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountTransactions_ShouldReturnCorrectData()
		{
			//Arrange
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;
			int page = 1;

			var expect = new TransactionsServiceModel
			{
				StartDate = startDate,
				EndDate = endDate
			};
			expect.Transactions = await data.Transactions
					.Where(t => t.AccountId == Account1User1.Id && t.CreatedOn >= startDate && t.CreatedOn <= endDate)
					.OrderByDescending(t => t.CreatedOn)
					.Take(TransactionsPerPage)
					.Select(t => new TransactionTableServiceModel
					{
						Id = t.Id,
						Amount = t.Amount,
						CreatedOn = t.CreatedOn,
						AccountCurrencyName = t.Account.Currency.Name,
						CategoryName = t.Category.Name + (t.Category.IsDeleted ?
							" (Deleted)"
							: string.Empty),
						Refference = t.Refference,
						TransactionType = t.TransactionType.ToString()
					})
					.ToListAsync();

			expect.TotalTransactionsCount = await data.Transactions.CountAsync(t => 
				t.AccountId == Account1User1.Id && t.CreatedOn >= startDate && t.CreatedOn <= endDate);

			//Act
			var actual = await accountService.GetAccountTransactions(Account1User1.Id, startDate, endDate, page);

			//Assert
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
				Assert.That(actual.Transactions.ElementAt(i).Refference,
					Is.EqualTo(expect.Transactions.ElementAt(i).Refference));
				Assert.That(actual.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(expect.Transactions.ElementAt(i).CategoryName));
				Assert.That(actual.Transactions.ElementAt(i).CreatedOn,
					Is.EqualTo(expect.Transactions.ElementAt(i).CreatedOn));
			}
		}

		[Test]
		public void GetAccountTransactions_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange
			string invalidId = Guid.NewGuid().ToString();
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;
			int page = 1;

			//Act & Assert
			Assert.That(async () => await accountService.GetAccountTransactions(invalidId, startDate, endDate, page),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionFormData_ShouldReturnCorrectData_WhenTransactionIsNotInitial()
		{
			//Arrange
			var orderedUserAccounts = await data.Accounts
				.Where(a => a.OwnerId == User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.ToListAsync();

			var orderedUserCategories = await data.Categories
				.Where(c => c.OwnerId == User1.Id && !c.IsDeleted)
				.OrderBy(c => c.Name)
				.ToListAsync();

			//Act
			TransactionFormServiceModel transactionFormModel =
				await accountService.GetTransactionFormData(Transaction2User1.Id);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			Assert.That(transactionFormModel.AccountId, Is.EqualTo(Transaction2User1.AccountId));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(Transaction2User1.Amount));
			Assert.That(transactionFormModel.CategoryId, Is.EqualTo(Transaction2User1.CategoryId));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(Transaction2User1.Refference));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(Transaction2User1.TransactionType));
			Assert.That(transactionFormModel.IsInitialBalance, Is.EqualTo(Transaction2User1.IsInitialBalance));
			Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(Transaction2User1.CreatedOn));
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
		}
		
		[Test]
		public async Task GetTransactionFormData_ShouldReturnCorrectData_WhenTransactionIsInitial()
		{
			//Arrange

			//Act
			TransactionFormServiceModel transactionFormModel =
				await accountService.GetTransactionFormData(Transaction1User1.Id);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			Assert.That(transactionFormModel.AccountId, Is.EqualTo(Transaction1User1.AccountId));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(Transaction1User1.Amount));
			Assert.That(transactionFormModel.CategoryId, Is.EqualTo(Transaction1User1.CategoryId));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(Transaction1User1.Refference));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(Transaction1User1.TransactionType));
			Assert.That(transactionFormModel.IsInitialBalance, Is.EqualTo(Transaction1User1.IsInitialBalance));
			Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(Transaction1User1.CreatedOn));
			Assert.That(transactionFormModel.UserAccounts.Count(), Is.EqualTo(1));
			Assert.That(transactionFormModel.UserAccounts.First().Name, Is.EqualTo(Account1User1.Name));
			Assert.That(transactionFormModel.UserCategories.Count(), Is.EqualTo(1));
			Assert.That(transactionFormModel.UserCategories.First().Name, Is.EqualTo(CategoryInitialBalanceName));
		}

		[Test]
		public void GetFulfilledTransactionFormModel_ShouldThrowException_WhenTransactionDoesNotExist()
		{
			//Act & Assert
			Assert.That(async () => await accountService.GetTransactionFormData(Guid.NewGuid().ToString()),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionDetailsServiceModel transactionFormModel =
				await accountService.GetTransactionDetails(Transaction1User1.Id, User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			Assert.That(transactionFormModel.Id, Is.EqualTo(Transaction1User1.Id));
			Assert.That(transactionFormModel.AccountName, Is.EqualTo(Transaction1User1.Account.Name));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(Transaction1User1.Amount));
			Assert.That(transactionFormModel.CategoryName, Is.EqualTo(Transaction1User1.Category.Name));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(Transaction1User1.Refference));
			Assert.That(transactionFormModel.OwnerId, Is.EqualTo(Transaction1User1.OwnerId));
			Assert.That(transactionFormModel.AccountCurrencyName, Is.EqualTo(Transaction1User1.Account.Currency.Name));
			Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(Transaction1User1.CreatedOn));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(Transaction1User1.TransactionType.ToString()));
		}
		
		[Test]
		public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WhenUserIsAdmin()
		{
			//Act
			TransactionDetailsServiceModel transactionFormModel =
				await accountService.GetTransactionDetails(Transaction1User1.Id, User2.Id, isUserAdmin: true);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			Assert.That(transactionFormModel.Id, Is.EqualTo(Transaction1User1.Id));
			Assert.That(transactionFormModel.AccountName, Is.EqualTo(Transaction1User1.Account.Name));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(Transaction1User1.Amount));
			Assert.That(transactionFormModel.CategoryName, Is.EqualTo(Transaction1User1.Category.Name));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(Transaction1User1.Refference));
			Assert.That(transactionFormModel.OwnerId, Is.EqualTo(Transaction1User1.OwnerId));
			Assert.That(transactionFormModel.AccountCurrencyName, Is.EqualTo(Transaction1User1.Account.Currency.Name));
			Assert.That(transactionFormModel.CreatedOn, Is.EqualTo(Transaction1User1.CreatedOn));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(Transaction1User1.TransactionType.ToString()));
		}

		[Test]
		public void GetTransactionDetails_ShouldThrowException_WithInValidTransactionId()
		{
			//Arrange
			string invalidId = Guid.NewGuid().ToString();

			//Act & Assert
			Assert.That(async () => await accountService.GetTransactionDetails(invalidId, User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}
		
		[Test]
		public void GetTransactionDetails_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await accountService.GetTransactionDetails(Transaction1User1.Id, User2.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo("User is not transaction's owner."));
		}

		[Test]
		public async Task IsAccountDeleted_ShouldReturnTrue_WhenAccountIsDeleted()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountDeleted(Account3User1Deleted.Id);

			//Assert
			Assert.That(response, Is.True);
		}

		[Test]
		public async Task IsAccountDeleted_ShouldReturnFalse_WhenAccountIsNotDeleted()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountDeleted(Account1User1.Id);

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
			bool response = await accountService.IsAccountOwner(User1.Id, Account1User1.Id);

			//Assert
			Assert.That(response, Is.True);
		}

		[Test]
		public async Task IsAccountOwner_ShouldReturnFalse_WhenUserIsNotOwner()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountOwner(User2.Id, Account1User1.Id);

			//Assert
			Assert.That(response, Is.False);
		}

		[Test]
		public void IsAccountOwner_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Assert
			Assert.That(async () => await accountService.IsAccountOwner(User1.Id, Guid.NewGuid().ToString()),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountShortDetails_ShouldReturnCorrectData_WithValidInput()
		{
			//Act
			var actualData = await accountService.GetAccountShortDetails(Account1User1.Id);

			//Assert
			Assert.That(actualData, Is.Not.Null);
			Assert.That(actualData.Name, Is.EqualTo(Account1User1.Name));
			Assert.That(actualData.Balance, Is.EqualTo(Account1User1.Balance));
			Assert.That(actualData.CurrencyName, Is.EqualTo(Account1User1.Currency.Name));
		}
		
		[Test]
		public void GetAccountShortDetails_ShouldThrowException_WithInvalidInput()
		{
			//Act & Assert
			Assert.That(async () => await accountService.GetAccountShortDetails(Guid.NewGuid().ToString()),
			Throws.TypeOf<InvalidOperationException>());			
		}
	}
}
