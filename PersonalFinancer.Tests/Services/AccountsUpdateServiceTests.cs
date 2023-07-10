namespace PersonalFinancer.Tests.Services
{
	using Microsoft.EntityFrameworkCore;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using static PersonalFinancer.Common.Constants.CategoryConstants;

	[TestFixture]
	internal class AccountsUpdateServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<Account> accountsRepo;
		private IEfRepository<Transaction> transactionsRepo;
		private IEfRepository<AccountType> accountTypesRepo;
		private IEfRepository<Currency> currenciesRepo;
		private IEfRepository<Category> categoriesRepo;

		private IAccountsUpdateService accountsUpdateService;

		[SetUp]
		public void SetUp()
		{
			this.accountsRepo = new EfRepository<Account>(this.dbContextMock);
			this.transactionsRepo = new EfRepository<Transaction>(this.dbContextMock);
			this.accountTypesRepo = new EfRepository<AccountType>(this.dbContextMock);
			this.currenciesRepo = new EfRepository<Currency>(this.dbContextMock);
			this.categoriesRepo = new EfRepository<Category>(this.dbContextMock);

			this.accountsUpdateService = new AccountsUpdateService(
				this.accountsRepo, this.transactionsRepo, this.accountTypesRepo,
				this.currenciesRepo, this.categoriesRepo, this.mapperMock);
		}

		[Test]
		public async Task CreateAccount_ShouldAddNewAccountAndTransaction_WithValidInput()
		{
			//Arrange
			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = "AccountWithNonZeroBalance",
				AccountTypeId = this.AccType1_User1_WithAcc.Id,
				Balance = 100,
				CurrencyId = this.Currency1_User1_WithAcc.Id,
				OwnerId = this.User1.Id
			};

			int accountsCountBefore = await this.accountsRepo.All().CountAsync();

			//Act
			Guid newAccountId = await this.accountsUpdateService.CreateAccountAsync(inputModel);
			Account newAccount = await this.accountsRepo.All()
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstAsync();
			Transaction initialBalanceTransaction = newAccount.Transactions.First();

			//Assert
			Assert.Multiple(async () =>
			{
				AssertSamePropertiesValuesAreEqual(newAccount, inputModel);

				Assert.That(await this.accountsRepo.All().CountAsync(), Is.EqualTo(accountsCountBefore + 1));
				Assert.That(newAccount.Transactions, Has.Count.EqualTo(1));
				Assert.That(initialBalanceTransaction.CategoryId, Is.EqualTo(this.Category_InitialBalance.Id));
				Assert.That(initialBalanceTransaction.IsInitialBalance, Is.True);
				Assert.That(initialBalanceTransaction.TransactionType, Is.EqualTo(TransactionType.Income));
				Assert.That(initialBalanceTransaction.Amount, Is.EqualTo(inputModel.Balance));
				Assert.That(initialBalanceTransaction.Reference, Is.EqualTo(this.Category_InitialBalance.Name));
			});
		}

		[Test]
		public async Task CreateAccount_ShouldAddNewAccountWithoutTransaction_WithValidInput()
		{
			//Arrange
			var newAccountModel = new CreateEditAccountOutputDTO
			{
				Name = "AccountWithZeroBalance",
				AccountTypeId = this.AccType1_User1_WithAcc.Id,
				Balance = 0,
				CurrencyId = this.Currency1_User1_WithAcc.Id,
				OwnerId = this.User1.Id
			};

			int accountsCountBefore = await this.accountsRepo.All().CountAsync();

			//Act
			Guid newAccountId = await this.accountsUpdateService.CreateAccountAsync(newAccountModel);
			Account newAccount = await this.accountsRepo.All()
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstAsync();

			//Assert
			Assert.Multiple(async () =>
			{
				AssertSamePropertiesValuesAreEqual(newAccount, newAccountModel);

				Assert.That(await this.accountsRepo.All().CountAsync(), Is.EqualTo(accountsCountBefore + 1));
				Assert.That(newAccount.Transactions.Any(), Is.False);
			});
		}

		[Test]
		public void CreateAccount_ShouldThrowException_WhenUserHaveAccountWithSameName()
		{
			//Arrange
			var newAccountModel = new CreateEditAccountOutputDTO
			{
				Name = this.Account1_User1_WithTransactions.Name,
				AccountTypeId = this.AccType1_User1_WithAcc.Id,
				Balance = 0,
				CurrencyId = this.Currency1_User1_WithAcc.Id,
				OwnerId = this.User1.Id
			};

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateAccountAsync(newAccountModel),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(
				string.Format(ExceptionMessages.ExistingUserEntityName, "account", this.Account1_User1_WithTransactions.Name)));
		}

		[Test]
		public void CreateAccount_ShouldThrowException_WhenAccountTypeIsNotValid()
		{
			//Arrange
			var newAccountModel = new CreateEditAccountOutputDTO
			{
				Name = "New Account With Invalid Account Type",
				AccountTypeId = Guid.NewGuid(),
				Balance = 0,
				CurrencyId = this.Currency1_User1_WithAcc.Id,
				OwnerId = this.User1.Id
			};

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateAccountAsync(newAccountModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidAccountType));
		}

		[Test]
		public void CreateAccount_ShouldThrowException_WhenCurrencyIsNotValid()
		{
			//Arrange
			var newAccountModel = new CreateEditAccountOutputDTO
			{
				Name = "New Account With Invalid Currency",
				AccountTypeId = this.AccType1_User1_WithAcc.Id,
				Balance = 0,
				CurrencyId = Guid.NewGuid(),
				OwnerId = this.User1.Id
			};

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateAccountAsync(newAccountModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCurrency));
		}

		[Test]
		[TestCase(TransactionType.Income)]
		[TestCase(TransactionType.Expense)]
		public async Task CreateTransaction_ShouldAddNewTransactionAndChangeAccountBalance(TransactionType transactionType)
		{
			//Arrange
			var account = this.Account1_User1_WithTransactions;
			var dto = new CreateEditTransactionOutputDTO()
			{
				Amount = 100,
				AccountId = account.Id,
				OwnerId = this.User1.Id,
				CategoryId = this.Category1_User1_WithTransactions.Id,
				CreatedOnLocalTime = DateTime.Now,
				Reference = "New transaction",
				TransactionType = transactionType
			};
			int transactionsCountBefore = account.Transactions.Count;
			decimal balanceBefore = account.Balance;

			//Act
			Guid id = await this.accountsUpdateService.CreateTransactionAsync(dto);
			Transaction? transaction = await this.transactionsRepo.FindAsync(id);

			//Assert
			Assert.That(transaction, Is.Not.Null);
			Assert.Multiple(() =>
			{
				if (transactionType == TransactionType.Income)
				{
					Assert.That(account.Transactions, Has.Count.EqualTo(transactionsCountBefore + 1));
					Assert.That(account.Balance, Is.EqualTo(balanceBefore + transaction.Amount));
				}
				else
				{
					Assert.That(account.Transactions, Has.Count.EqualTo(transactionsCountBefore + 1));
					Assert.That(account.Balance, Is.EqualTo(balanceBefore - transaction.Amount));
				}

				AssertSamePropertiesValuesAreEqual(transaction, dto);
			});
		}

		[Test]
		public void CreateTransaction_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange
			var dto = this.mapperMock.Map<CreateEditTransactionOutputDTO>(this.Transaction1_Expense_Account1_User1);
			dto.AccountId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateTransactionAsync(dto),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void CreateTransaction_ShouldThrowException_WhenCategoryIsNotValid()
		{
			//Arrange
			var dto = this.mapperMock.Map<CreateEditTransactionOutputDTO>(this.Transaction1_Expense_Account1_User1);
			dto.CategoryId = Guid.NewGuid();

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateTransactionAsync(dto),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCategory));
		}

		[Test]
		public async Task DeleteAccount_ShouldDeleteAccountWithoutTransactions_WithValidId()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			int accountTransactionsCount = account.Transactions.Count;

			//Act
			await this.accountsUpdateService.DeleteAccountAsync(
				account.Id, this.User1.Id, isUserAdmin: false, shouldDeleteTransactions: false);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(account.IsDeleted, Is.True);

				Assert.That(account.Transactions, 
				   Has.Count.EqualTo(accountTransactionsCount));
			});
		}

		[Test]
		public async Task DeleteAccount_ShouldDeleteAccountAndTransactions_WithValidId()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			int accountTransactionsCount = account.Transactions.Count;
			int accountsCountBefore = await this.accountsRepo.All().CountAsync();
			int transactionsCountBefore = await this.transactionsRepo.All().CountAsync();

			//Act
			await this.accountsUpdateService.DeleteAccountAsync(
				account.Id, this.User1.Id, isUserAdmin: false, shouldDeleteTransactions: true);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(await this.accountsRepo.All().CountAsync(), 
					Is.EqualTo(accountsCountBefore - 1));

				Assert.That(await this.transactionsRepo.All().CountAsync(), 
					Is.EqualTo(transactionsCountBefore - accountTransactionsCount));
			});
		}

		[Test]
		public void DeleteAccount_ShouldThrowException_WhenIdIsInvalid()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService
				  .DeleteAccountAsync(Guid.NewGuid(), this.User1.Id, isUserAdmin: false, shouldDeleteTransactions: true),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void DeleteAccount_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange
			Guid accountId = this.Account1_User1_WithTransactions.Id;

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService
				  .DeleteAccountAsync(accountId, this.User2.Id, isUserAdmin: false, shouldDeleteTransactions: true),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public async Task DeleteAccount_ShouldDeleteAccountAndTransactions_WhenUserIsAdmin()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			int accountTransactionsCount = account.Transactions.Count;
			int accountsCountBefore = await this.accountsRepo.All().CountAsync();
			int transactionsCountBefore = await this.transactionsRepo.All().CountAsync();

			//Act
			await this.accountsUpdateService.DeleteAccountAsync(
				account.Id, this.User2.Id, isUserAdmin: true, shouldDeleteTransactions: true);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(await this.accountsRepo.All().CountAsync(),
					Is.EqualTo(accountsCountBefore - 1));

				Assert.That(await this.transactionsRepo.All().CountAsync(), 
					Is.EqualTo(transactionsCountBefore - accountTransactionsCount));
			});
		}

		[Test]
		public async Task DeleteAccount_ShouldDeleteAccountWithoutTransactions_WhenUserIsAdmin()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			int accountsCountBefore = await this.accountsRepo.All().CountAsync();
			int transactionsCountBefore = await this.transactionsRepo.All().CountAsync();

			//Act
			await this.accountsUpdateService.DeleteAccountAsync(
				account.Id, this.User2.Id, isUserAdmin: true, shouldDeleteTransactions: false);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(account.IsDeleted, Is.True);
				
				Assert.That(await this.accountsRepo.All().CountAsync(),
					Is.EqualTo(accountsCountBefore));

				Assert.That(await this.transactionsRepo.All().CountAsync(), 
					Is.EqualTo(transactionsCountBefore));
			});
		}

		[Test]
		public async Task DeleteTransaction_ShouldDeleteIncomeTransactionAndDecreaseBalance_WithValidInput()
		{
			//Arrange
			Transaction transaction = this.Transaction3_Income_Account3_User1;
			Account account = this.Account3_User1_Deleted_WithTransactions;
			decimal balanceBefore = account.Balance;
			int accountTransactionsCountBefore = account.Transactions.Count;

			//Act
			decimal newBalance = await this.accountsUpdateService
				.DeleteTransactionAsync(transaction.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(account.Balance, Is.EqualTo(balanceBefore - transaction.Amount));
				Assert.That(account.Balance, Is.EqualTo(newBalance));
				Assert.That(account.Transactions, Has.Count.EqualTo(accountTransactionsCountBefore - 1));
				Assert.That(await this.transactionsRepo.FindAsync(transaction.Id), Is.Null);
			});
		}

		[Test]
		public async Task DeleteTransaction_ShouldDeleteIncomeTransactionAndDecreaseBalance_WhenUserIsAdmin()
		{
			//Arrange
			Transaction transaction = this.Transaction3_Income_Account3_User1;
			Account account = this.Account3_User1_Deleted_WithTransactions;
			decimal balanceBefore = account.Balance;
			int accountTransactionsCountBefore = account.Transactions.Count;

			//Act
			decimal newBalance = await this.accountsUpdateService
				.DeleteTransactionAsync(transaction.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(account.Balance, Is.EqualTo(balanceBefore - transaction.Amount));
				Assert.That(account.Balance, Is.EqualTo(newBalance));
				Assert.That(account.Transactions, Has.Count.EqualTo(accountTransactionsCountBefore - 1));
				Assert.That(await this.transactionsRepo.FindAsync(transaction.Id), Is.Null);
			});
		}

		[Test]
		public async Task DeleteTransaction_ShouldDeleteExpenseTransactionIncreaseBalanceAndNewBalance_WithValidInput()
		{
			//Arrange
			Transaction transaction = this.Transaction1_Expense_Account1_User1;
			Account account = this.Account1_User1_WithTransactions;
			decimal balanceBefore = account.Balance;
			int accountTransactionsCountBefore = account.Transactions.Count;

			//Act
			decimal newBalance = await this.accountsUpdateService
				.DeleteTransactionAsync(transaction.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(account.Balance, Is.EqualTo(balanceBefore + transaction.Amount));
				Assert.That(account.Balance, Is.EqualTo(newBalance));
				Assert.That(account.Transactions, Has.Count.EqualTo(accountTransactionsCountBefore - 1));
				Assert.That(await this.transactionsRepo.FindAsync(transaction.Id), Is.Null);
			});
		}

		[Test]
		public async Task DeleteTransaction_ShouldDeleteExpenseTransactionIncreaseBalanceAndNewBalance_WhenUserIsAdmin()
		{
			//Arrange
			Transaction transaction = this.Transaction1_Expense_Account1_User1;
			Account account = this.Account1_User1_WithTransactions;
			decimal balanceBefore = account.Balance;
			int accountTransactionsCountBefore = account.Transactions.Count;

			//Act
			decimal newBalance = await this.accountsUpdateService
				.DeleteTransactionAsync(transaction.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(balanceBefore + transaction.Amount));
				Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(newBalance));
				Assert.That(this.Account1_User1_WithTransactions.Transactions, Has.Count.EqualTo(accountTransactionsCountBefore - 1));
				Assert.That(await this.transactionsRepo.FindAsync(transaction.Id), Is.Null);
			});
		}

		[Test]
		public void DeleteTransaction_ShouldThrowAnException_WithInvalidInput()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService
				  .DeleteTransactionAsync(Guid.NewGuid(), this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void DeleteTransaction_ShouldThrowAnException_WithUserIsNotOwner()
		{
			//Arrange
			Guid transactionId = this.Transaction1_Expense_Account1_User1.Id;

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService
				  .DeleteTransactionAsync(transactionId, this.User2.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public async Task EditAccount_ShouldChangeAccountName()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = "New Name", // Change
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance,
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel);

			//Assert
			AssertSamePropertiesValuesAreEqual(account, inputModel);
		}

		[Test]
		public async Task EditAccount_ShouldChangeCurrency()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = account.Name,
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance,
				CurrencyId = this.Currency2_User1_WithoutAcc.Id, // Change
				OwnerId = account.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel);

			//Assert
			AssertSamePropertiesValuesAreEqual(account, inputModel);
		}

		[Test]
		public async Task EditAccount_ShouldChangeAccountAccountType()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = account.Name,
				AccountTypeId = this.AccType2_User1_WithoutAcc.Id, // Change
				Balance = account.Balance,
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel);

			//Assert
			AssertSamePropertiesValuesAreEqual(account, inputModel);
		}

		[Test]
		public async Task EditAccount_ShouldChangeAccountBalanceAndInitialBalanceTransactionAmount()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			Transaction initialTransaction = this.InitialTransaction_Income_Account1_User1;
			decimal initialTransactionAmountBefore = initialTransaction.Amount;

			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = account.Name,
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance + 100, // Change
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(initialTransaction.Amount, Is.EqualTo(initialTransactionAmountBefore + 100));
				Assert.That(initialTransaction.TransactionType, Is.EqualTo(TransactionType.Income));

				AssertSamePropertiesValuesAreEqual(account, inputModel);
			});
		}

		[Test]
		public async Task EditAccount_ShouldChangeAccountBalanceAndInitialBalanceTransactionAmountAndType()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			Transaction initialTransaction = this.InitialTransaction_Income_Account1_User1;
			decimal initialTransactionAmountBefore = initialTransaction.Amount;

			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = account.Name,
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance - 1000, // Change
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(initialTransaction.Amount, Is.EqualTo(initialTransactionAmountBefore - 1000));
				Assert.That(initialTransaction.TransactionType, Is.EqualTo(TransactionType.Expense));

				AssertSamePropertiesValuesAreEqual(account, inputModel);
			});
		}

		[Test]
		public async Task EditAccount_ShouldChangeAccountBalanceAndCreateInitialBalanceTransaction()
		{
			//Arrange
			Account account = this.Account2_User1_WithoutTransactions;
			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = account.Name,
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance + 100, // Change
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel);

			Transaction initialBalTransaction = account.Transactions.First(t => t.IsInitialBalance);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(initialBalTransaction.Amount, Is.EqualTo(100));
				Assert.That(initialBalTransaction.IsInitialBalance, Is.True);
				Assert.That(initialBalTransaction.Reference, Is.EqualTo(CategoryInitialBalanceName));
				Assert.That(initialBalTransaction.OwnerId, Is.EqualTo(account.OwnerId));
				Assert.That(initialBalTransaction.AccountId, Is.EqualTo(account.Id));
				Assert.That(initialBalTransaction.CategoryId, Is.EqualTo(this.Category_InitialBalance.Id));
				Assert.That(initialBalTransaction.TransactionType, Is.EqualTo(TransactionType.Income));

				Assert.That(account.Balance, Is.EqualTo(100));
				AssertSamePropertiesValuesAreEqual(account, inputModel);
			});
		}

		[Test]
		public void EditAccount_ShouldThrowExceptionWhenUserHaveAccountWithSameName()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;
			string anotherAccountName = this.Account2_User1_WithoutTransactions.Name;

			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = anotherAccountName, // Change
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance,
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel),
			Throws.TypeOf<ArgumentException>().With.Message
				  .EqualTo(string.Format(ExceptionMessages.ExistingUserEntityName, "account", anotherAccountName)));
		}

		[Test]
		public void EditAccount_ShouldThrowException_WhenAccountTypeIsNotValid()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;

			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = account.Name,
				AccountTypeId = Guid.NewGuid(), // Invalid Id
				Balance = account.Balance,
				CurrencyId = account.CurrencyId,
				OwnerId = account.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidAccountType));
		}

		[Test]
		public void EditAccount_ShouldThrowException_WhenCurrencyIsNotValid()
		{
			//Arrange
			Account account = this.Account1_User1_WithTransactions;

			var inputModel = new CreateEditAccountOutputDTO
			{
				Name = account.Name,
				AccountTypeId = account.AccountTypeId,
				Balance = account.Balance,
				CurrencyId = Guid.NewGuid(), // Invalid Id
				OwnerId = account.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCurrency));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalance_WhenTransactionTypeIsChanged()
		{
			//Arrange
			var account = this.Account1_User1_WithTransactions;
			var transaction = this.Transaction1_Expense_Account1_User1;
			decimal balanceBefore = account.Balance;
			var dto = this.mapperMock.Map<CreateEditTransactionOutputDTO>(transaction);

			//Act
			dto.TransactionType = TransactionType.Income;
			await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto);

			//Assert
			Assert.Multiple(() =>
			{
				AssertSamePropertiesValuesAreEqual(transaction, dto);

				Assert.That(account.Balance,
					Is.EqualTo(balanceBefore + (transaction.Amount * 2)));
			});
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalanceOnTwoAccounts_WhenAccountIsChanged()
		{
			//Arrange
			var firstAccount = this.Account1_User1_WithTransactions;
			var secondAccount = this.Account2_User1_WithoutTransactions;
			decimal firstAccBalanceBefore = firstAccount.Balance;
			decimal secondAccBalanceBefore = secondAccount.Balance;
			var transaction = this.Transaction1_Expense_Account1_User1;
			var dto = this.mapperMock.Map<CreateEditTransactionOutputDTO>(transaction);

			//Act
			dto.AccountId = secondAccount.Id;
			await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(transaction.AccountId,
					Is.EqualTo(secondAccount.Id));

				Assert.That(firstAccount.Balance,
					Is.EqualTo(firstAccBalanceBefore + transaction.Amount));

				Assert.That(secondAccount.Balance,
					Is.EqualTo(secondAccBalanceBefore - transaction.Amount));
			});
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransaction_WhenPaymentReferenceIsChanged()
		{
			//Arrange
			var account = this.Account1_User1_WithTransactions;
			decimal balanceBefore = account.Balance;
			var transaction = this.Transaction1_Expense_Account1_User1;
			var dto = this.mapperMock.Map<CreateEditTransactionOutputDTO>(transaction);

			//Act
			dto.Reference = "Second Reference";
			await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(account.Balance, Is.EqualTo(balanceBefore));

				AssertSamePropertiesValuesAreEqual(account, dto);
			});
		}

		[Test]
		public void EditTransaction_ShouldThrowException_WhenCategoryIsNotValid()
		{
			//Arrange
			var account = this.Account1_User1_WithTransactions;
			var transaction = this.Transaction1_Expense_Account1_User1;
			var dto = this.mapperMock.Map<CreateEditTransactionOutputDTO>(transaction);

			//Act
			dto.CategoryId = Guid.NewGuid();

			//Assert
			Assert.That(async () => await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCategory));
		}

		[Test]
		public void EditTransaction_ShouldThrowException_WhenTransactionIsInitial()
		{
			//Arrange
			var account = this.Account1_User1_WithTransactions;
			var transaction = this.InitialTransaction_Income_Account1_User1;
			var dto = this.mapperMock.Map<CreateEditTransactionOutputDTO>(transaction);

			//Act
			dto.CategoryId = this.Category1_User1_WithTransactions.Id;

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.EditInitialTransaction));
		}
	}
}
