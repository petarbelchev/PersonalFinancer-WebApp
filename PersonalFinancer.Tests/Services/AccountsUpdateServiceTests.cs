namespace PersonalFinancer.Tests.Services
{
	using Microsoft.EntityFrameworkCore;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Constants;
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
		private Guid mainTestAccountTypeId;
		private Guid mainTestCurrencyId;
		private Guid mainTestCategoryId;
		private Guid mainTestAccountId;

		private IEfRepository<Account> accountsRepo;
		private IEfRepository<Transaction> transactionsRepo;
		private IEfRepository<AccountType> accountTypesRepo;
		private IEfRepository<Currency> currenciesRepo;
		private IEfRepository<Category> categoriesRepo;

		private IAccountsUpdateService accountsUpdateService;

		[SetUp]
		public async Task SetUp()
		{
			this.accountsRepo = new EfRepository<Account>(this.dbContext);
			this.transactionsRepo = new EfRepository<Transaction>(this.dbContext);
			this.accountTypesRepo = new EfRepository<AccountType>(this.dbContext);
			this.currenciesRepo = new EfRepository<Currency>(this.dbContext);
			this.categoriesRepo = new EfRepository<Category>(this.dbContext);

			this.accountsUpdateService = new AccountsUpdateService(
				this.accountsRepo,
				this.transactionsRepo,
				this.accountTypesRepo,
				this.currenciesRepo,
				this.categoriesRepo,
				this.mapper,
				this.cacheMock.Object);

			this.mainTestAccountTypeId = await this.accountTypesRepo.All()
				.Where(at => at.OwnerId == this.mainTestUserId && !at.IsDeleted)
				.Select(at => at.Id)
				.FirstAsync();

			this.mainTestCurrencyId = await this.currenciesRepo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
				.Select(at => at.Id)
				.FirstAsync();

			this.mainTestCategoryId = await this.categoriesRepo.All()
				.Where(at => at.OwnerId == this.mainTestUserId && !at.IsDeleted)
				.Select(at => at.Id)
				.FirstAsync();

			this.mainTestAccountId = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.Select(at => at.Id)
				.FirstAsync();
		}

		[Test]
		[TestCase(100)]
		[TestCase(-100)]
		public async Task CreateAccountAsync_ShouldAddNewAccountAndInitialBalanceTransaction_WhenInputIsValid(decimal balance)
		{
			//Arrange
			var inputModel = new CreateEditAccountInputDTO
			{
				Name = "AccountWithNonZeroBalance",
				AccountTypeId = this.mainTestAccountTypeId,
				Balance = balance,
				CurrencyId = this.mainTestCurrencyId,
				OwnerId = this.mainTestUserId
			};

			int accountsCountBefore = await this.accountsRepo.All().CountAsync();

			var expectedTransactionType = balance < 0
				? TransactionType.Expense
				: TransactionType.Income;

			string cacheKey = CacheConstants.AccountsAndCategoriesKey + this.mainTestUserId;

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
				Assert.That(initialBalanceTransaction.CategoryId, Is.EqualTo(Guid.Parse(InitialBalanceCategoryId)));
				Assert.That(initialBalanceTransaction.IsInitialBalance, Is.True);
				Assert.That(initialBalanceTransaction.TransactionType, Is.EqualTo(expectedTransactionType));
				Assert.That(initialBalanceTransaction.Amount, Is.EqualTo(inputModel.Balance));
				Assert.That(initialBalanceTransaction.Reference, Is.EqualTo(CategoryInitialBalanceName));
			});

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
		}

		[Test]
		public async Task CreateAccountAsync_ShouldAddNewAccountWithoutTransaction_WhenInputIsValid()
		{
			//Arrange
			var inputModel = new CreateEditAccountInputDTO
			{
				Name = "AccountWithZeroBalance",
				AccountTypeId = this.mainTestAccountTypeId,
				Balance = 0,
				CurrencyId = this.mainTestCurrencyId,
				OwnerId = this.mainTestUserId
			};

			int accountsCountBefore = await this.accountsRepo.All().CountAsync();

			string cacheKey = CacheConstants.AccountsAndCategoriesKey + this.mainTestUserId;

			//Act
			Guid newAccountId = await this.accountsUpdateService.CreateAccountAsync(inputModel);
			Account newAccount = await this.accountsRepo.All()
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstAsync();

			//Assert
			Assert.Multiple(async () =>
			{
				AssertSamePropertiesValuesAreEqual(newAccount, inputModel);

				Assert.That(await this.accountsRepo.All().CountAsync(), Is.EqualTo(accountsCountBefore + 1));
				Assert.That(newAccount.Transactions.Any(), Is.False);
			});

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
		}

		[Test]
		public async Task CreateAccountAsync_ShouldThrowArgumentException_WhenUserHaveAccountWithTheSameName()
		{
			//Arrange
			string existingName = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId)
				.Select(a => a.Name)
				.FirstAsync();

			var inputModel = new CreateEditAccountInputDTO
			{
				Name = existingName,
				AccountTypeId = this.mainTestAccountTypeId,
				Balance = 0,
				CurrencyId = this.mainTestCurrencyId,
				OwnerId = this.mainTestUserId
			};

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateAccountAsync(inputModel),
			Throws.TypeOf<ArgumentException>().With.Message
				  .EqualTo(string.Format(ExceptionMessages.ExistingUserEntityName, "account", existingName)));
		}

		[Test]
		public void CreateAccountAsync_ShouldThrowInvalidOperationException_WhenTheAccountTypeIsNotValid()
		{
			//Arrange
			var newAccountModel = new CreateEditAccountInputDTO
			{
				Name = "New Account With Invalid Account Type",
				AccountTypeId = Guid.NewGuid(),
				Balance = 0,
				CurrencyId = this.mainTestCurrencyId,
				OwnerId = this.mainTestUserId
			};

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateAccountAsync(newAccountModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidAccountType));
		}

		[Test]
		public void CreateAccountAsync_ShouldThrowInvalidOperationException_WhenTheCurrencyIsNotValid()
		{
			//Arrange
			var newAccountModel = new CreateEditAccountInputDTO
			{
				Name = "New Account With Invalid Currency",
				AccountTypeId = this.mainTestAccountTypeId,
				Balance = 0,
				CurrencyId = Guid.NewGuid(),
				OwnerId = this.mainTestUserId
			};

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateAccountAsync(newAccountModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCurrency));
		}

		[Test]
		[TestCase(TransactionType.Income)]
		[TestCase(TransactionType.Expense)]
		public async Task CreateTransactionAsync_ShouldAddNewTransactionAndChangeAccountBalance(TransactionType transactionType)
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.Include(a => a.Transactions)
				.FirstAsync();

			var inputDto = new CreateEditTransactionInputDTO()
			{
				Amount = 100,
				AccountId = testAccount.Id,
				OwnerId = this.mainTestUserId,
				CategoryId = this.mainTestCategoryId,
				CreatedOnLocalTime = DateTime.Now,
				Reference = "New transaction",
				TransactionType = transactionType
			};
			int transactionsCountBefore = testAccount.Transactions.Count;
			decimal balanceBefore = testAccount.Balance;

			//Act
			Guid id = await this.accountsUpdateService.CreateTransactionAsync(inputDto);
			Transaction? transaction = await this.transactionsRepo.FindAsync(id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(transaction, Is.Not.Null);
				Assert.That(testAccount.Transactions, Has.Count.EqualTo(transactionsCountBefore + 1));

				if (transactionType == TransactionType.Income)
					Assert.That(testAccount.Balance, Is.EqualTo(balanceBefore + transaction!.Amount));
				else
					Assert.That(testAccount.Balance, Is.EqualTo(balanceBefore - transaction!.Amount));

				AssertSamePropertiesValuesAreEqual(transaction, inputDto);
			});
		}

		[Test]
		public void CreateTransactionAsync_ShouldThrowInvalidOperationException_WhenTheAccountDoesNotExist()
		{
			//Arrange
			var inputDto = new CreateEditTransactionInputDTO()
			{
				Amount = 100,
				AccountId = Guid.NewGuid(),
				OwnerId = this.mainTestUserId,
				CategoryId = this.mainTestCategoryId,
				CreatedOnLocalTime = DateTime.Now,
				Reference = "New transaction",
				TransactionType = TransactionType.Expense
			};

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateTransactionAsync(inputDto),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void CreateTransactionAsync_ShouldThrowInvalidOperationException_WhenTheCategoryDoesNotExist()
		{
			//Arrange
			var inputDto = new CreateEditTransactionInputDTO()
			{
				Amount = 100,
				AccountId = this.mainTestAccountId,
				OwnerId = this.mainTestUserId,
				CategoryId = Guid.NewGuid(),
				CreatedOnLocalTime = DateTime.Now,
				Reference = "New transaction",
				TransactionType = TransactionType.Expense
			};

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateTransactionAsync(inputDto),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCategory));
		}

		[Test]
		[TestCase(false, false)]
		[TestCase(true, false)]
		[TestCase(true, true)]
		[TestCase(false, true)]
		public async Task DeleteAccountAsync_ShouldMarkAccountAsDeletedWithoutDeleteTransactions(bool isUserAdmin, bool shouldDeleteTransactions)
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.Include(a => a.Transactions)
				.FirstAsync();

			int accountTransactionsCount = testAccount.Transactions.Count;
			int accountsCountBefore = await this.accountsRepo.All().CountAsync();
			int transactionsCountBefore = await this.transactionsRepo.All().CountAsync();

			Guid currentUserId = isUserAdmin ? this.adminId : testAccount.OwnerId;

			string cacheKey = CacheConstants.AccountsAndCategoriesKey + this.mainTestUserId;

			//Act
			await this.accountsUpdateService.DeleteAccountAsync(
				testAccount.Id, currentUserId, isUserAdmin, shouldDeleteTransactions);

			//Assert
			if (shouldDeleteTransactions)
			{
				Assert.Multiple(async () =>
				{
					Assert.That(await this.accountsRepo.All().CountAsync(),
						Is.EqualTo(accountsCountBefore - 1));

					Assert.That(await this.transactionsRepo.All().CountAsync(),
						Is.EqualTo(transactionsCountBefore - accountTransactionsCount));
				});
			}
			else
			{
				Assert.Multiple(async () =>
				{
					Assert.That(testAccount.IsDeleted, Is.True);

					Assert.That(testAccount.Transactions,
					   Has.Count.EqualTo(accountTransactionsCount));

					Assert.That(await this.accountsRepo.All().CountAsync(),
						Is.EqualTo(accountsCountBefore));

					Assert.That(await this.transactionsRepo.All().CountAsync(),
						Is.EqualTo(transactionsCountBefore));
				});
			}

			this.cacheMock.Verify(x => x.Remove(cacheKey), Times.Once);
		}

		[Test]
		public void DeleteAccountAsync_ShouldThrowInvalidOperationException_WhenTheAccountDoesNotExist()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService
				  .DeleteAccountAsync(Guid.NewGuid(), this.mainTestUserId, isUserAdmin: false, shouldDeleteTransactions: true),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteAccountAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			Guid notOwnerId = await this.dbContext.Users
				.Where(u => u.Id != this.mainTestUserId)
				.Select(u => u.Id)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.DeleteAccountAsync(
				this.mainTestAccountId,
				notOwnerId,
				isUserAdmin: false,
				shouldDeleteTransactions: true),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		[TestCase(false, TransactionType.Income)]
		[TestCase(false, TransactionType.Expense)]
		[TestCase(true, TransactionType.Income)]
		[TestCase(true, TransactionType.Expense)]
		public async Task DeleteTransactionAsync_ShouldDeleteTransactionAndChangeTheBalance(bool isUserAdmin, TransactionType transactionType)
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId &&
							!a.IsDeleted &&
							a.Transactions.Any(t => t.TransactionType == transactionType))
				.Include(a => a.Transactions)
				.FirstAsync();

			Transaction transactionForDelete = testAccount.Transactions
				.First(t => t.TransactionType == transactionType);

			decimal balanceBefore = testAccount.Balance;

			decimal expectedNewBalance = transactionType == TransactionType.Income
				? balanceBefore - transactionForDelete.Amount
				: balanceBefore + transactionForDelete.Amount;

			int accountTransactionsCountBefore = testAccount.Transactions.Count;

			Guid currentUserId = isUserAdmin ? this.adminId : testAccount.OwnerId;

			//Act
			decimal newBalance = await this.accountsUpdateService
				.DeleteTransactionAsync(transactionForDelete.Id, currentUserId, isUserAdmin);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(testAccount.Balance,
					Is.EqualTo(expectedNewBalance));

				Assert.That(testAccount.Balance,
					Is.EqualTo(newBalance));

				Assert.That(testAccount.Transactions,
				   Has.Count.EqualTo(accountTransactionsCountBefore - 1));

				Assert.That(await this.transactionsRepo.FindAsync(transactionForDelete.Id),
					Is.Null);
			});
		}

		[Test]
		public void DeleteTransactionAsync_ShouldThrowInvalidOperationException_WhenTheTransactionDoesNotExist()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService
				  .DeleteTransactionAsync(Guid.NewGuid(), this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteTransactionAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			Guid transactionId = await this.transactionsRepo.All()
				.Where(t => t.OwnerId != this.mainTestUserId)
				.Select(t => t.Id)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService
				  .DeleteTransactionAsync(transactionId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public async Task EditAccountAsync_ShouldChangeAccountName()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => !a.IsDeleted)
				.FirstAsync();

			var inputModel = new CreateEditAccountInputDTO
			{
				Name = "New Name", // Change
				AccountTypeId = testAccount.AccountTypeId,
				Balance = testAccount.Balance,
				CurrencyId = testAccount.CurrencyId,
				OwnerId = testAccount.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(testAccount.Id, inputModel);

			//Assert
			AssertSamePropertiesValuesAreEqual(testAccount, inputModel);
		}

		[Test]
		public async Task EditAccountAsync_ShouldChangeCurrency()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => !a.IsDeleted)
				.FirstAsync();

			Guid newCurrencyId = await this.currenciesRepo.All()
				.Where(c => c.Id != testAccount.CurrencyId &&
							c.OwnerId == testAccount.OwnerId &&
							!c.IsDeleted)
				.Select(c => c.Id)
				.FirstAsync();

			var inputModel = new CreateEditAccountInputDTO
			{
				Name = testAccount.Name,
				AccountTypeId = testAccount.AccountTypeId,
				Balance = testAccount.Balance,
				CurrencyId = newCurrencyId, // Change
				OwnerId = testAccount.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(testAccount.Id, inputModel);

			//Assert
			AssertSamePropertiesValuesAreEqual(testAccount, inputModel);
		}

		[Test]
		public async Task EditAccountAsync_ShouldChangeAccountAccountType()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.FirstAsync();

			Guid newAccountTypeId = await this.accountTypesRepo.All()
				.Where(at => !at.IsDeleted &&
							  at.OwnerId == this.mainTestUserId &&
							  at.Id != testAccount.AccountTypeId)
				.Select(at => at.Id)
				.FirstAsync();

			var inputModel = new CreateEditAccountInputDTO
			{
				Name = testAccount.Name,
				AccountTypeId = newAccountTypeId, // Change
				Balance = testAccount.Balance,
				CurrencyId = testAccount.CurrencyId,
				OwnerId = testAccount.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(testAccount.Id, inputModel);

			//Assert
			AssertSamePropertiesValuesAreEqual(testAccount, inputModel);
		}

		[Test]
		public async Task EditAccountAsync_ShouldChangeAccountBalanceAndInitialBalanceTransactionAmountAndType()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => !a.IsDeleted
							&& a.Transactions.Any(t => t.IsInitialBalance
													   && t.TransactionType == TransactionType.Income))
				.FirstAsync();

			Transaction initialTransaction = testAccount.Transactions.First(t => t.IsInitialBalance);
			decimal initialTransactionAmountBefore = initialTransaction.Amount;
			decimal balanceBefore = testAccount.Balance;

			var inputModel = new CreateEditAccountInputDTO
			{
				Name = testAccount.Name,
				AccountTypeId = testAccount.AccountTypeId,
				Balance = -1000, // Change
				CurrencyId = testAccount.CurrencyId,
				OwnerId = testAccount.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(testAccount.Id, inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(initialTransaction.Amount, Is.EqualTo(initialTransactionAmountBefore - (balanceBefore + 1000)));
				Assert.That(initialTransaction.TransactionType, Is.EqualTo(TransactionType.Expense));

				AssertSamePropertiesValuesAreEqual(testAccount, inputModel);
			});
		}

		[Test]
		public async Task EditAccountAsync_ShouldChangeAccountBalanceAndCreateInitialBalanceTransaction()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => !a.IsDeleted &&
							!a.Transactions.Any(t => t.IsInitialBalance))
				.FirstAsync();

			var inputModel = new CreateEditAccountInputDTO
			{
				Name = testAccount.Name,
				AccountTypeId = testAccount.AccountTypeId,
				Balance = testAccount.Balance + 100, // Change
				CurrencyId = testAccount.CurrencyId,
				OwnerId = testAccount.OwnerId
			};

			//Act
			await this.accountsUpdateService.EditAccountAsync(testAccount.Id, inputModel);
			Transaction initialBalTransaction = testAccount.Transactions.First(t => t.IsInitialBalance);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(initialBalTransaction.Amount, Is.EqualTo(100));
				Assert.That(initialBalTransaction.IsInitialBalance, Is.True);
				Assert.That(initialBalTransaction.Reference, Is.EqualTo(CategoryInitialBalanceName));
				Assert.That(initialBalTransaction.CategoryId, Is.EqualTo(Guid.Parse(InitialBalanceCategoryId)));
				Assert.That(initialBalTransaction.OwnerId, Is.EqualTo(testAccount.OwnerId));
				Assert.That(initialBalTransaction.AccountId, Is.EqualTo(testAccount.Id));
				Assert.That(initialBalTransaction.TransactionType, Is.EqualTo(TransactionType.Income));

				Assert.That(testAccount.Balance, Is.EqualTo(100));
				AssertSamePropertiesValuesAreEqual(testAccount, inputModel);
			});
		}

		[Test]
		public async Task EditAccountAsync_ShouldThrowArgumentException_WhenTheUserHaveAccountWithTheSameName()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.FirstAsync();

			string existingName = await this.accountsRepo.All()
				.Where(a => a.Id != testAccount.Id &&
							a.OwnerId == this.mainTestUserId)
				.Select(a => a.Name)
				.FirstAsync();

			var inputModel = new CreateEditAccountInputDTO
			{
				Name = existingName, // Change
				AccountTypeId = testAccount.AccountTypeId,
				Balance = testAccount.Balance,
				CurrencyId = testAccount.CurrencyId,
				OwnerId = testAccount.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditAccountAsync(testAccount.Id, inputModel),
			Throws.TypeOf<ArgumentException>().With.Message
				  .EqualTo(string.Format(ExceptionMessages.ExistingUserEntityName, "account", existingName)));
		}

		[Test]
		public async Task EditAccountAsync_ShouldThrowInvalidOperationException_WhenTheAccountTypeIsInvalid()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.FirstAsync();

			var inputModel = new CreateEditAccountInputDTO
			{
				Name = testAccount.Name,
				AccountTypeId = Guid.NewGuid(), // Invalid Id
				Balance = testAccount.Balance,
				CurrencyId = testAccount.CurrencyId,
				OwnerId = testAccount.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditAccountAsync(testAccount.Id, inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidAccountType));
		}

		[Test]
		public async Task EditAccountAsync_ShouldThrowInvalidOperationException_WhenTheCurrencyIsInvalid()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.FirstAsync();

			var inputModel = new CreateEditAccountInputDTO
			{
				Name = testAccount.Name,
				AccountTypeId = testAccount.AccountTypeId,
				Balance = testAccount.Balance,
				CurrencyId = Guid.NewGuid(), // Invalid Id
				OwnerId = testAccount.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditAccountAsync(testAccount.Id, inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCurrency));
		}

		[Test]
		public async Task EditTransactionAsync_ShouldEditTransactionAndChangeBalance_WhenTheTransactionTypeIsChanged()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.FirstAsync();

			Transaction transaction = testAccount.Transactions
				.First(t => t.TransactionType == TransactionType.Expense &&
							!t.IsInitialBalance);

			decimal balanceBefore = testAccount.Balance;
			var dto = this.mapper.Map<CreateEditTransactionInputDTO>(transaction);

			//Act
			dto.TransactionType = TransactionType.Income;
			await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto);

			//Assert
			Assert.Multiple(() =>
			{
				AssertSamePropertiesValuesAreEqual(transaction, dto);

				Assert.That(testAccount.Balance,
					Is.EqualTo(balanceBefore + (transaction.Amount * 2)));
			});
		}

		[Test]
		public async Task EditTransactionAsync_ShouldEditTransactionAndChangeBalanceOnBothAccounts_WhenTheAccountIsChanged()
		{
			//Arrange
			Account firstAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId &&
							!a.IsDeleted)
				.FirstAsync();

			Account secondAccount = await this.accountsRepo.All()
				.Where(a => a.Id != firstAccount.Id &&
							a.OwnerId == this.mainTestUserId &&
							!a.IsDeleted)
				.FirstAsync();

			Transaction transaction = firstAccount.Transactions
				.First(t => t.TransactionType == TransactionType.Expense &&
							!t.IsInitialBalance);

			var dto = this.mapper.Map<CreateEditTransactionInputDTO>(transaction);

			decimal firstAccBalanceBefore = firstAccount.Balance;
			decimal secondAccBalanceBefore = secondAccount.Balance;

			//Act
			dto.AccountId = secondAccount.Id;
			await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto);

			//Assert
			Assert.Multiple(() =>
			{
				AssertSamePropertiesValuesAreEqual(transaction, dto);

				Assert.That(firstAccount.Balance,
					Is.EqualTo(firstAccBalanceBefore + transaction.Amount));

				Assert.That(secondAccount.Balance,
					Is.EqualTo(secondAccBalanceBefore - transaction.Amount));
			});
		}

		[Test]
		public async Task EditTransactionAsync_ShouldEditTransaction_WhenThePaymentReferenceIsChanged()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId && !a.IsDeleted)
				.FirstAsync();

			Transaction transaction = testAccount.Transactions.First(t => !t.IsInitialBalance);
			var dto = this.mapper.Map<CreateEditTransactionInputDTO>(transaction);

			decimal balanceBefore = testAccount.Balance;

			//Act
			dto.Reference = "Second Reference";
			await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(testAccount.Balance, Is.EqualTo(balanceBefore));

				AssertSamePropertiesValuesAreEqual(transaction, dto);
			});
		}

		[Test]
		public async Task EditTransactionAsync_ShouldThrowInvalidOperationException_WhenTheCategoryIsInvalid()
		{
			//Arrange
			Transaction transaction = await this.transactionsRepo.All()
				.FirstAsync(t => !t.IsInitialBalance);

			var dto = this.mapper.Map<CreateEditTransactionOutputDTO>(transaction);

			//Act
			dto.CategoryId = Guid.NewGuid();

			//Assert
			Assert.That(async () => await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCategory));
		}

		[Test]
		public async Task EditTransactionAsync_ShouldThrowInvalidOperationException_WhenTheTransactionIsInitial()
		{
			//Arrange
			Transaction transaction = await this.transactionsRepo.All()
				.FirstAsync(t => t.IsInitialBalance && t.CategoryId != this.mainTestCategoryId);

			var dto = this.mapper.Map<CreateEditTransactionOutputDTO>(transaction);

			//Act
			dto.CategoryId = this.mainTestCategoryId;

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.EditInitialTransaction));
		}
	}
}
