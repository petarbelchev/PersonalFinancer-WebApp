﻿namespace PersonalFinancer.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Enums;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using static PersonalFinancer.Data.Constants.CategoryConstants;

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
            this.accountsRepo = new EfRepository<Account>(this.sqlDbContext);
            this.transactionsRepo = new EfRepository<Transaction>(this.sqlDbContext);
            this.accountTypesRepo = new EfRepository<AccountType>(this.sqlDbContext);
            this.currenciesRepo = new EfRepository<Currency>(this.sqlDbContext);
            this.categoriesRepo = new EfRepository<Category>(this.sqlDbContext);

			this.accountsUpdateService = new AccountsUpdateService(
                this.accountsRepo, this.transactionsRepo, this.accountTypesRepo, 
                this.currenciesRepo, this.categoriesRepo, this.mapper);
        }

        [Test]
        public async Task CreateAccount_ShouldAddNewAccountAndTransaction_WithValidInput()
        {
            //Arrange
            var inputModel = new CreateEditAccountDTO
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
                Assert.That(await this.accountsRepo.All().CountAsync(), Is.EqualTo(accountsCountBefore + 1));
                Assert.That(newAccount.Name, Is.EqualTo(inputModel.Name));
                Assert.That(newAccount.Balance, Is.EqualTo(inputModel.Balance));
                Assert.That(newAccount.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
                Assert.That(newAccount.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
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
            var newAccountModel = new CreateEditAccountDTO
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
                Assert.That(await this.accountsRepo.All().CountAsync(), Is.EqualTo(accountsCountBefore + 1));
                Assert.That(newAccount.Name, Is.EqualTo(newAccountModel.Name));
                Assert.That(newAccount.Balance, Is.EqualTo(newAccountModel.Balance));
                Assert.That(newAccount.CurrencyId, Is.EqualTo(newAccountModel.CurrencyId));
                Assert.That(newAccount.AccountTypeId, Is.EqualTo(newAccountModel.AccountTypeId));
                Assert.That(newAccount.Transactions.Any(), Is.False);
            });
        }

        [Test]
        public void CreateAccount_ShouldThrowException_WhenUserHaveAccountWithSameName()
        {
            //Arrange
            var newAccountModel = new CreateEditAccountDTO
            {
                Name = this.Account1_User1_WithTransactions.Name,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                Balance = 0,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                OwnerId = this.User1.Id
            };

            // Act & Assert
            Assert.That(async () => await this.accountsUpdateService.CreateAccountAsync(newAccountModel),
            Throws.TypeOf<ArgumentException>().With.Message
                  .EqualTo(string.Format(ExceptionMessages.ExistingUserEntityName, "account", this.Account1_User1_WithTransactions.Name)));
        }

		[Test]
		public void CreateAccount_ShouldThrowException_WhenAccountTypeIsNotValid()
		{
			//Arrange
			var newAccountModel = new CreateEditAccountDTO
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
			var newAccountModel = new CreateEditAccountDTO
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
			var dto = new CreateEditTransactionDTO()
            {
                Amount = 100,
                AccountId = account.Id,
                OwnerId = this.User1.Id,
                CategoryId = this.Category1_User1_WithTransactions.Id,
                CreatedOn = DateTime.UtcNow,
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

                Assert.That(transaction.Amount, Is.EqualTo(dto.Amount));
                Assert.That(transaction.CategoryId, Is.EqualTo(dto.CategoryId));
                Assert.That(transaction.AccountId, Is.EqualTo(dto.AccountId));
                Assert.That(transaction.Reference, Is.EqualTo(dto.Reference));
                Assert.That(transaction.CreatedOn, Is.EqualTo(dto.CreatedOn));
                Assert.That(transaction.TransactionType, Is.EqualTo(dto.TransactionType));
            });
        }

        [Test]
        public void CreateTransaction_ShouldThrowException_WhenAccountDoesNotExist()
        {
            //Arrange
            var dto = this.mapper.Map<CreateEditTransactionDTO>(this.Transaction1_Expense_Account1_User1);
            dto.AccountId = Guid.NewGuid();

            //Act & Assert
            Assert.That(async () => await this.accountsUpdateService.CreateTransactionAsync(dto),
            Throws.TypeOf<InvalidOperationException>());
        }

		[Test]
		public void CreateTransaction_ShouldThrowException_WhenCategoryIsNotValid()
		{
			//Arrange
			var dto = this.mapper.Map<CreateEditTransactionDTO>(this.Transaction1_Expense_Account1_User1);
			dto.CategoryId = Guid.NewGuid();

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.CreateTransactionAsync(dto),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCategory));
		}

		[Test]
        public async Task DeleteAccount_ShouldDeleteAccountWithoutTransactions_WithValidId()
        {
            //Arrange
            var accId = Guid.NewGuid();
            await this.accountsRepo.AddAsync(new Account
            {
                Id = accId,
                Name = "AccountForDelete",
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                Balance = 0,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                OwnerId = this.User1.Id,
                Transactions = new HashSet<Transaction>
                {
                    new Transaction
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = this.User1.Id,
                        Amount = 200,
                        CategoryId = this.Category_InitialBalance.Id,
                        CreatedOn = DateTime.UtcNow,
                        Reference = "Salary",
                        TransactionType = TransactionType.Income
                    }
                }
            });
            await this.accountsRepo.SaveChangesAsync();

            //Assert that the Account and Transactions are created and Account is not deleted
            Account? account = await this.accountsRepo.FindAsync(accId);
            Assert.That(account, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(account.IsDeleted, Is.False);
                Assert.That(account.Transactions, Has.Count.EqualTo(1));
            });

            //Act
            await this.accountsUpdateService.DeleteAccountAsync(accId, this.User1.Id, isUserAdmin: false, shouldDeleteTransactions: false);

            //Assert that the Account is mark as deleted but Transactions not
            Assert.Multiple(() =>
            {
                Assert.That(account.IsDeleted, Is.True);
                Assert.That(account.Transactions, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public async Task DeleteAccount_ShouldDeleteAccountAndTransactions_WithValidId()
        {
            //Arrange
            var accountId = Guid.NewGuid();
            await this.accountsRepo.AddAsync(new Account
            {
                Id = accountId,
                Name = "AccountForDelete",
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                Balance = 0,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                OwnerId = this.User1.Id,
                Transactions = new HashSet<Transaction>
                {
                    new Transaction
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = this.User1.Id,
                        Amount = 200,
                        CategoryId = this.Category_InitialBalance.Id,
                        CreatedOn = DateTime.UtcNow,
                        Reference = "Salary",
                        TransactionType = TransactionType.Income
                    }
                }
            });
            await this.accountsRepo.SaveChangesAsync();

            //Assert that the Account and Transactions are created and Account is not deleted
            Account? account = await this.accountsRepo.FindAsync(accountId);
            Assert.That(account, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(account.IsDeleted, Is.False);
                Assert.That(account.Transactions, Has.Count.EqualTo(1));
            });

            //Arrange
            int accountsCountBefore = await this.accountsRepo.All().CountAsync();
            int transactionsCountBefore = await this.transactionsRepo.All().CountAsync();

            //Act
            await this.accountsUpdateService.DeleteAccountAsync(accountId, this.User1.Id, isUserAdmin: false, shouldDeleteTransactions: true);

            //Assert that the Account is deleted but Transactions not
            Assert.Multiple(async () =>
            {
                Assert.That(await this.accountsRepo.All().CountAsync(), Is.EqualTo(accountsCountBefore - 1));
                Assert.That(await this.transactionsRepo.All().CountAsync(), Is.EqualTo(transactionsCountBefore - 1));
            });
        }

        [Test]
        public void DeleteAccount_ShouldThrowException_WhenIdIsInvalid()
        {
            //Arrange & Act

            //Assert
            Assert.That(async () => await this.accountsUpdateService
                  .DeleteAccountAsync(Guid.NewGuid(), this.User1.Id, isUserAdmin: false, shouldDeleteTransactions: true),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task DeleteAccount_ShouldThrowException_WhenUserIsNotOwner()
        {
            //Arrange
            var accId = Guid.NewGuid();
            await this.accountsRepo.AddAsync(new Account
            {
                Id = accId,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                Balance = 0,
                Name = "For Delete",
                OwnerId = this.User1.Id
            });
            await this.accountsRepo.SaveChangesAsync();

            //Act & Assert
            Assert.That(async () => await this.accountsUpdateService
                  .DeleteAccountAsync(accId, this.User2.Id, isUserAdmin: false, shouldDeleteTransactions: true),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
        }

        [Test]
        public async Task DeleteAccount_ShouldDeleteAccountAndTransactions_WhenUserIsAdmin()
        {
            //Arrange
            var accId = Guid.NewGuid();
            await this.accountsRepo.AddAsync(new Account
            {
                Id = accId,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                Balance = 0,
                Name = "For Delete 2",
                OwnerId = this.User1.Id,
                Transactions = new HashSet<Transaction>
                {
                    new Transaction
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = this.User1.Id,
                        Amount = 200,
                        CategoryId = this.Category_InitialBalance.Id,
                        CreatedOn = DateTime.UtcNow,
                        Reference = "Salary",
                        TransactionType = TransactionType.Income
                    }
                }
            });
            await this.accountsRepo.SaveChangesAsync();

            //Assert that the Account and Transactions are created and Account is not deleted
            Account? account = await this.accountsRepo.FindAsync(accId);
            Assert.That(account, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(account.IsDeleted, Is.False);
                Assert.That(account.Transactions, Has.Count.EqualTo(1));
            });

            //Arrange
            int accountsCountBefore = await this.accountsRepo.All().CountAsync();
            int transactionsCountBefore = await this.transactionsRepo.All().CountAsync();

            //Act
            await this.accountsUpdateService.DeleteAccountAsync(accId, this.User2.Id, isUserAdmin: true, shouldDeleteTransactions: true);

            //Assert that the Account is deleted but Transactions not
            Assert.Multiple(async () =>
            {
                Assert.That(await this.accountsRepo.All().CountAsync(), Is.EqualTo(accountsCountBefore - 1));
                Assert.That(await this.transactionsRepo.All().CountAsync(), Is.EqualTo(transactionsCountBefore - 1));
            });
        }

        [Test]
        public async Task DeleteAccount_ShouldDeleteAccountWithoutTransactions_WhenUserIsAdmin()
        {
            //Arrange
            var accId = Guid.NewGuid();
            await this.accountsRepo.AddAsync(new Account
            {
                Id = accId,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                Balance = 0,
                Name = "For Delete 3",
                OwnerId = this.User1.Id,
                Transactions = new HashSet<Transaction>
                {
                    new Transaction
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = this.User1.Id,
                        Amount = 200,
                        CategoryId = this.Category_InitialBalance.Id,
                        CreatedOn = DateTime.UtcNow,
                        Reference = "Salary",
                        TransactionType = TransactionType.Income
                    }
                }
            });
            await this.accountsRepo.SaveChangesAsync();

            //Assert that the Account and Transactions are created and Account is not deleted
            Account? account = await this.accountsRepo.FindAsync(accId);
            Assert.That(account, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(account.IsDeleted, Is.False);
                Assert.That(account.Transactions, Has.Count.EqualTo(1));
            });

            //Arrange
            int accountsCountBefore = await this.accountsRepo.All().CountAsync();
            int transactionsCountBefore = await this.transactionsRepo.All().CountAsync();

            //Act
            await this.accountsUpdateService.DeleteAccountAsync(accId, this.User2.Id, isUserAdmin: true, shouldDeleteTransactions: false);

            //Assert that the Account is deleted but Transactions not
            Assert.Multiple(async () =>
            {
                Assert.That(account.IsDeleted, Is.True);
                Assert.That(await this.accountsRepo.All().CountAsync(), Is.EqualTo(accountsCountBefore));
                Assert.That(await this.transactionsRepo.All().CountAsync(), Is.EqualTo(transactionsCountBefore));
            });
        }

        [Test]
        public async Task DeleteTransaction_ShouldDeleteIncomeTransactionAndDecreaseBalance_WithValidInput()
        {
            //Arrange
            var transactionId = Guid.NewGuid();
            var transaction = new Transaction
            {
                Id = transactionId,
                OwnerId = this.User1.Id,
                AccountId = this.Account1_User1_WithTransactions.Id,
                Amount = 123,
                CategoryId = this.Category_InitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "TestTransaction",
                TransactionType = TransactionType.Income
            };
            await this.transactionsRepo.AddAsync(transaction);
            this.Account1_User1_WithTransactions.Balance += transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();

            decimal balanceBefore = this.Account1_User1_WithTransactions.Balance;
            int transactionsBefore = this.Account1_User1_WithTransactions.Transactions.Count;
            Transaction? transactionInDb = await this.transactionsRepo.FindAsync(transactionId);

            //Assert
            Assert.That(transactionInDb, Is.Not.Null);

            //Act
            decimal newBalance = await this.accountsUpdateService.DeleteTransactionAsync(transactionId, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(balanceBefore - transactionInDb.Amount));
                Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(newBalance));
                Assert.That(this.Account1_User1_WithTransactions.Transactions, Has.Count.EqualTo(transactionsBefore - 1));
                Assert.That(await this.transactionsRepo.FindAsync(transactionId), Is.Null);
            });
        }

        [Test]
        public async Task DeleteTransaction_ShouldDeleteIncomeTransactionAndDecreaseBalance_WhenUserIsAdmin()
        {
            //Arrange
            var transactionId = Guid.NewGuid();
            var transaction = new Transaction
            {
                Id = transactionId,
                OwnerId = this.User1.Id,
                AccountId = this.Account1_User1_WithTransactions.Id,
                Amount = 123,
                CategoryId = this.Category_InitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "TestTransaction",
                TransactionType = TransactionType.Income
            };
            await this.transactionsRepo.AddAsync(transaction);
            this.Account1_User1_WithTransactions.Balance += transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();

            decimal balanceBefore = this.Account1_User1_WithTransactions.Balance;
            int transactionsBefore = this.Account1_User1_WithTransactions.Transactions.Count;
            Transaction? transactionInDb = await this.transactionsRepo.FindAsync(transactionId);

            //Assert
            Assert.That(transactionInDb, Is.Not.Null);

            //Act
            decimal newBalance = await this.accountsUpdateService.DeleteTransactionAsync(transactionId, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(balanceBefore - transactionInDb.Amount));
                Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(newBalance));
                Assert.That(this.Account1_User1_WithTransactions.Transactions, Has.Count.EqualTo(transactionsBefore - 1));
                Assert.That(await this.transactionsRepo.FindAsync(transactionId), Is.Null);
            });
        }

        [Test]
        public async Task DeleteTransaction_ShouldDeleteExpenseTransactionIncreaseBalanceAndNewBalance_WithValidInput()
        {
            //Arrange
            var transactionId = Guid.NewGuid();
            var transaction = new Transaction
            {
                Id = transactionId,
                AccountId = this.Account1_User1_WithTransactions.Id,
                OwnerId = this.User1.Id,
                Amount = 123,
                CategoryId = this.Category_InitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "TestTransaction",
                TransactionType = TransactionType.Expense
            };
            await this.transactionsRepo.AddAsync(transaction);
            this.Account1_User1_WithTransactions.Balance -= transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();

            decimal balanceBefore = this.Account1_User1_WithTransactions.Balance;
            int transactionsBefore = this.Account1_User1_WithTransactions.Transactions.Count;
            Transaction? transactionInDb = await this.transactionsRepo.FindAsync(transactionId);

            //Assert
            Assert.That(transactionInDb, Is.Not.Null);

            //Act
            decimal newBalance = await this.accountsUpdateService.DeleteTransactionAsync(transactionId, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
                Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(newBalance));
                Assert.That(this.Account1_User1_WithTransactions.Transactions, Has.Count.EqualTo(transactionsBefore - 1));
                Assert.That(await this.transactionsRepo.FindAsync(transactionId), Is.Null);
            });
        }

        [Test]
        public async Task DeleteTransaction_ShouldDeleteExpenseTransactionIncreaseBalanceAndNewBalance_WhenUserIsAdmin()
        {
            //Arrange
            var transactionId = Guid.NewGuid();
            var transaction = new Transaction
            {
                Id = transactionId,
                AccountId = this.Account1_User1_WithTransactions.Id,
                OwnerId = this.User1.Id,
                Amount = 123,
                CategoryId = this.Category_InitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "TestTransaction",
                TransactionType = TransactionType.Expense
            };
            await this.transactionsRepo.AddAsync(transaction);
            this.Account1_User1_WithTransactions.Balance -= transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();

            decimal balanceBefore = this.Account1_User1_WithTransactions.Balance;
            int transactionsBefore = this.Account1_User1_WithTransactions.Transactions.Count;
            Transaction? transactionInDb = await this.transactionsRepo.FindAsync(transactionId);

            //Assert
            Assert.That(transactionInDb, Is.Not.Null);

            //Act
            decimal newBalance = await this.accountsUpdateService.DeleteTransactionAsync(transactionId, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
                Assert.That(this.Account1_User1_WithTransactions.Balance, Is.EqualTo(newBalance));
                Assert.That(this.Account1_User1_WithTransactions.Transactions, Has.Count.EqualTo(transactionsBefore - 1));
                Assert.That(await this.transactionsRepo.FindAsync(transactionId), Is.Null);
            });
        }

        [Test]
        public void DeleteTransaction_ShouldThrowAnException_WithInvalidInput()
        {
            //Arrange

            //Act & Assert
            Assert.That(async () => await this.accountsUpdateService.DeleteTransactionAsync(Guid.NewGuid(), this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task DeleteTransaction_ShouldThrowAnException_WithUserIsNotOwner()
        {
            //Arrange
            var id = Guid.NewGuid();
            var transaction = new Transaction
            {
                Id = id,
                AccountId = this.Account1_User1_WithTransactions.Id,
                CategoryId = this.Currency1_User1_WithAcc.Id,
                Amount = 10,
                CreatedOn = DateTime.UtcNow,
                OwnerId = this.User1.Id,
                Reference = "For Delete",
                TransactionType = TransactionType.Expense
            };
            await this.transactionsRepo.AddAsync(transaction);
            await this.transactionsRepo.SaveChangesAsync();

            //Act & Assert
            Assert.That(async () => await this.accountsUpdateService.DeleteTransactionAsync(id, this.User2.Id, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
        }

        [Test]
        public async Task EditAccount_ShouldChangeAccountName()
        {
            //Arrange
            var accId = Guid.NewGuid();
            var account = new Account
            {
                Id = accId,
                Name = "For Edit",
                Balance = 10,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                OwnerId = this.User1.Id
            };
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            var inputModel = new CreateEditAccountDTO
            {
                Name = "New Name", // Change
                AccountTypeId = account.AccountTypeId,
                Balance = account.Balance,
                CurrencyId = account.CurrencyId,
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountsUpdateService.EditAccountAsync(accId, inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(account.Name, Is.EqualTo(inputModel.Name));
                Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
                Assert.That(account.Balance, Is.EqualTo(inputModel.Balance));
                Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
                Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
            });
        }

        [Test]
        public async Task EditAccount_ShouldChangeCurrency()
        {
            //Arrange
            var accId = Guid.NewGuid();
            var account = new Account
            {
                Id = accId,
                Name = "For Edit 2",
                Balance = 10,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                OwnerId = this.User1.Id
            };
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            var inputModel = new CreateEditAccountDTO
            {
                Name = account.Name,
                AccountTypeId = account.AccountTypeId,
                Balance = account.Balance,
                CurrencyId = this.Currency2_User1_WithoutAcc.Id, // Change
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountsUpdateService.EditAccountAsync(accId, inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(account.Name, Is.EqualTo(inputModel.Name));
                Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
                Assert.That(account.Balance, Is.EqualTo(inputModel.Balance));
                Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
                Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
            });
        }

        [Test]
        public async Task EditAccount_ShouldChangeAccountAccountType()
        {
            //Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Name = "For Edit 3",
                Balance = 10,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                OwnerId = this.User1.Id
            };
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            var inputModel = new CreateEditAccountDTO
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
            Assert.Multiple(() =>
            {
                Assert.That(account.Name, Is.EqualTo(inputModel.Name));
                Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
                Assert.That(account.Balance, Is.EqualTo(inputModel.Balance));
                Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
                Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
            });
        }

        [Test]
        public async Task EditAccount_ShouldChangeAccountBalanceAndInitialBalanceTransactionAmount()
        {
            //Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Name = "For Edit 4",
                Balance = 5,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                OwnerId = this.User1.Id
            };
            var initialBalTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = 10,
                OwnerId = account.OwnerId,
                AccountId = account.Id,
                CategoryId = this.Category_InitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "Initial Balance",
                IsInitialBalance = true,
                TransactionType = TransactionType.Income
            };
            var expenseTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = 5,
                OwnerId = account.OwnerId,
                AccountId = account.Id,
                CategoryId = this.Category_InitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "Lunch",
                TransactionType = TransactionType.Expense
            };
            account.Transactions.Add(initialBalTransaction);
            account.Transactions.Add(expenseTransaction);
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            decimal initBalTransactionAmountBefore = initialBalTransaction.Amount;

            decimal accountExpensesSum = account.Transactions
                .Where(t => t.TransactionType == TransactionType.Expense)
                .Sum(t => t.Amount);

            decimal accountIncomesSum = account.Transactions
                .Where(t => t.TransactionType == TransactionType.Income)
                .Sum(t => t.Amount);

            //Assert that the account has correct balance before the test
            Assert.That(account.Balance, Is.EqualTo(accountIncomesSum - accountExpensesSum));

            var inputModel = new CreateEditAccountDTO
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
                Assert.That(account.Balance, Is.EqualTo(accountIncomesSum - accountExpensesSum + 100));
                Assert.That(account.Balance, Is.EqualTo(inputModel.Balance));
                Assert.That(initialBalTransaction.Amount, Is.EqualTo(initBalTransactionAmountBefore + 100));
                Assert.That(initialBalTransaction.TransactionType, Is.EqualTo(TransactionType.Income));
                Assert.That(account.Name, Is.EqualTo(inputModel.Name));
                Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
                Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
                Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
                Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
            });
        }

        [Test]
        public async Task EditAccount_ShouldChangeAccountBalanceAndInitialBalanceTransactionAmountAndType()
        {
            //Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Name = "For Edit 4",
                Balance = 5,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                OwnerId = this.User1.Id
            };
            var initialBalTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = 10,
                OwnerId = account.OwnerId,
                AccountId = account.Id,
                CategoryId = this.Category_InitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "Initial Balance",
                IsInitialBalance = true,
                TransactionType = TransactionType.Income
            };
            var expenseTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = 5,
                OwnerId = account.OwnerId,
                AccountId = account.Id,
                CategoryId = this.Category_InitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "Lunch",
                TransactionType = TransactionType.Expense
            };
            account.Transactions.Add(initialBalTransaction);
            account.Transactions.Add(expenseTransaction);
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            decimal initBalTransactionAmountBefore = initialBalTransaction.Amount;

            decimal accountExpensesSum = account.Transactions
                .Where(t => t.TransactionType == TransactionType.Expense)
                .Sum(t => t.Amount);

            decimal accountIncomesSum = account.Transactions
                .Where(t => t.TransactionType == TransactionType.Income)
                .Sum(t => t.Amount);

            //Assert that the account has correct balance before the test
            Assert.That(account.Balance, Is.EqualTo(accountIncomesSum - accountExpensesSum));

            var inputModel = new CreateEditAccountDTO
            {
                Name = account.Name,
                AccountTypeId = account.AccountTypeId,
                Balance = account.Balance - 100, // Change
                CurrencyId = account.CurrencyId,
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(account.Balance, Is.EqualTo(accountIncomesSum - accountExpensesSum - 100));
                Assert.That(account.Balance, Is.EqualTo(inputModel.Balance));
                Assert.That(initialBalTransaction.Amount, Is.EqualTo(initBalTransactionAmountBefore - 100));
                Assert.That(initialBalTransaction.TransactionType, Is.EqualTo(TransactionType.Expense));
                Assert.That(account.Name, Is.EqualTo(inputModel.Name));
                Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
                Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
                Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
            });
        }

        [Test]
        public async Task EditAccount_ShouldChangeAccountBalanceAndCreateInitialBalanceTransaction()
        {
            //Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Name = "For Edit 3",
                Balance = 0,
                AccountTypeId = this.AccType1_User1_WithAcc.Id,
                CurrencyId = this.Currency1_User1_WithAcc.Id,
                OwnerId = this.User1.Id
            };
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            var inputModel = new CreateEditAccountDTO
            {
                Name = account.Name,
                AccountTypeId = account.AccountTypeId,
                Balance = account.Balance + 100, // Change
                CurrencyId = account.CurrencyId,
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountsUpdateService.EditAccountAsync(account.Id, inputModel);

            Transaction initialBalTransaction =
                account.Transactions.First(t => t.IsInitialBalance);

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
                Assert.That(account.Balance, Is.EqualTo(inputModel.Balance));
                Assert.That(account.Name, Is.EqualTo(inputModel.Name));
                Assert.That(account.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
                Assert.That(account.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
                Assert.That(account.OwnerId, Is.EqualTo(inputModel.OwnerId));
            });
        }

        [Test]
        public void EditAccount_ShouldThrowExceptionWhenUserHaveAccountWithSameName()
        {
            //Arrange
            var inputModel = new CreateEditAccountDTO
            {
                Name = this.Account2_User1_WithoutTransactions.Name, // Change
                AccountTypeId = this.Account1_User1_WithTransactions.AccountTypeId,
                Balance = this.Account1_User1_WithTransactions.Balance,
                CurrencyId = this.Account1_User1_WithTransactions.CurrencyId,
                OwnerId = this.Account1_User1_WithTransactions.OwnerId
            };

            //Act & Assert
            Assert.That(async () => await this.accountsUpdateService.EditAccountAsync(this.Account1_User1_WithTransactions.Id, inputModel),
            Throws.TypeOf<ArgumentException>().With.Message
                  .EqualTo(string.Format(ExceptionMessages.ExistingUserEntityName, "account", this.Account2_User1_WithoutTransactions.Name)));
        }

		[Test]
		public void EditAccount_ShouldThrowException_WhenAccountTypeIsNotValid()
		{
			//Arrange
			var inputModel = new CreateEditAccountDTO
			{
				Name = this.Account1_User1_WithTransactions.Name,
				AccountTypeId = Guid.NewGuid(), // Invalid Id
				Balance = this.Account1_User1_WithTransactions.Balance,
				CurrencyId = this.Account1_User1_WithTransactions.CurrencyId,
				OwnerId = this.Account1_User1_WithTransactions.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditAccountAsync(this.Account1_User1_WithTransactions.Id, inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidAccountType));
		}

		[Test]
		public void EditAccount_ShouldThrowException_WhenCurrencyIsNotValid()
		{
			//Arrange
			var inputModel = new CreateEditAccountDTO
			{
				Name = this.Account1_User1_WithTransactions.Name,
				AccountTypeId = this.Account1_User1_WithTransactions.AccountTypeId,
				Balance = this.Account1_User1_WithTransactions.Balance,
				CurrencyId = Guid.NewGuid(), // Invalid Id
				OwnerId = this.Account1_User1_WithTransactions.OwnerId
			};

			//Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditAccountAsync(this.Account1_User1_WithTransactions.Id, inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.InvalidCurrency));
		}

		[Test]
        public async Task EditTransaction_ShouldEditTransactionAndChangeBalance_WhenTransactionTypeIsChanged()
        {
            //Arrange
            var account = this.Account1_User1_WithTransactions;
			var transaction = this.Transaction1_Expense_Account1_User1;
			decimal balanceBefore = account.Balance;
            var dto = this.mapper.Map<CreateEditTransactionDTO>(transaction);

			//Act
			dto.TransactionType = TransactionType.Income;
            await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(transaction.TransactionType,
                    Is.EqualTo(dto.TransactionType));

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
			var dto = this.mapper.Map<CreateEditTransactionDTO>(transaction);

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
            var dto = this.mapper.Map<CreateEditTransactionDTO>(transaction);

            //Act
            dto.Reference = "Second Reference";
            await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto);

            //Assert that only transaction reference is changed
            Assert.Multiple(() =>
            {
                Assert.That(account.Balance, Is.EqualTo(balanceBefore));
                Assert.That(transaction.Reference, Is.EqualTo(dto.Reference));
                Assert.That(transaction.CategoryId, Is.EqualTo(dto.CategoryId));
                Assert.That(transaction.AccountId, Is.EqualTo(dto.AccountId));
                Assert.That(transaction.Amount, Is.EqualTo(dto.Amount));
                Assert.That(transaction.OwnerId, Is.EqualTo(dto.OwnerId));
                Assert.That(transaction.CreatedOn, Is.EqualTo(dto.CreatedOn.ToUniversalTime()));
            });
        }

		[Test]
		public void EditTransaction_ShouldThrowException_WhenCategoryIsNotValid()
		{
			//Arrange
			var account = this.Account1_User1_WithTransactions;
			var transaction = this.Transaction1_Expense_Account1_User1;
			var dto = this.mapper.Map<CreateEditTransactionDTO>(transaction);

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
			var dto = this.mapper.Map<CreateEditTransactionDTO>(transaction);

            //Act
            dto.CategoryId = this.Category1_User1_WithTransactions.Id;

			// Act & Assert
			Assert.That(async () => await this.accountsUpdateService.EditTransactionAsync(transaction.Id, dto),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.EditInitialTransaction));
		}
	}
}