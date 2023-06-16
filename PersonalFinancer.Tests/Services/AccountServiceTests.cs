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
    using static PersonalFinancer.Services.Infrastructure.Constants.PaginationConstants;

    [TestFixture]
    internal class AccountServiceTests : ServicesUnitTestsBase
    {
        private IEfRepository<Account> accountsRepo;
        private IEfRepository<Transaction> transactionsRepo;
        private IEfRepository<Category> categoriesRepo;
        private IAccountsService accountService;

        [SetUp]
        public void SetUp()
        {
            this.accountsRepo = new EfRepository<Account>(this.sqlDbContext);
            this.transactionsRepo = new EfRepository<Transaction>(this.sqlDbContext);
            this.categoriesRepo = new EfRepository<Category>(this.sqlDbContext);
            this.accountService = new AccountsService(this.accountsRepo, this.transactionsRepo, this.mapper, this.memoryCache);
        }

        [Test]
        public async Task CreateAccount_ShouldAddNewAccountAndTransaction_WithValidInput()
        {
            //Arrange
            var inputModel = new AccountFormShortServiceModel
            {
                Name = "AccountWithNonZeroBalance",
                AccountTypeId = this.AccType1User1.Id,
                Balance = 100,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id
            };

            int accountsCountBefore = await this.accountsRepo.All().CountAsync();

            //Act
            Guid newAccountId = await this.accountService.CreateAccountAsync(inputModel);
            Account newAccount = await this.accountsRepo.All()
                .Where(a => a.Id == newAccountId)
                .Include(a => a.Transactions)
                .FirstAsync();

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(await this.accountsRepo.All().CountAsync(), Is.EqualTo(accountsCountBefore + 1));
                Assert.That(newAccount.Name, Is.EqualTo(inputModel.Name));
                Assert.That(newAccount.Balance, Is.EqualTo(inputModel.Balance));
                Assert.That(newAccount.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
                Assert.That(newAccount.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
                Assert.That(newAccount.Transactions, Has.Count.EqualTo(1));
                Assert.That(newAccount.Transactions.First().CategoryId, Is.EqualTo(this.CatInitialBalance.Id));
            });
        }

        [Test]
        public async Task CreateAccount_ShouldAddNewAccountWithoutTransaction_WithValidInput()
        {
            //Arrange
            var newAccountModel = new AccountFormShortServiceModel
            {
                Name = "AccountWithZeroBalance",
                AccountTypeId = this.AccType1User1.Id,
                Balance = 0,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id
            };

            int accountsCountBefore = await this.accountsRepo.All().CountAsync();

            //Act
            Guid newAccountId = await this.accountService.CreateAccountAsync(newAccountModel);
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
            var newAccountModel = new AccountFormShortServiceModel
            {
                Name = this.Account1User1.Name,
                AccountTypeId = this.AccType1User1.Id,
                Balance = 0,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id
            };

            // Act & Assert
            Assert.That(async () => await this.accountService.CreateAccountAsync(newAccountModel),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(
                $"The User already have Account with \"{this.Account1User1.Name}\" name."));
        }

        [Test]
        public async Task CreateTransaction_ShouldAddNewTransaction_AndDecreaseAccountBalance()
        {
            //Arrange
            var transactionModel = new TransactionFormShortServiceModel()
            {
                Amount = 100,
                AccountId = this.Account1User1.Id,
                OwnerId = this.User1.Id,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "Not Initial Balance",
                TransactionType = TransactionType.Expense
            };
            int transactionsCountBefore = this.Account1User1.Transactions.Count;
            decimal balanceBefore = this.Account1User1.Balance;

            //Act
            Guid id = await this.accountService.CreateTransactionAsync(transactionModel);
            Transaction? transaction = await this.transactionsRepo.FindAsync(id);

            //Assert
            Assert.That(transaction, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(this.Account1User1.Transactions, Has.Count.EqualTo(transactionsCountBefore + 1));
                Assert.That(transaction.Amount, Is.EqualTo(transactionModel.Amount));
                Assert.That(transaction.CategoryId, Is.EqualTo(transactionModel.CategoryId));
                Assert.That(transaction.AccountId, Is.EqualTo(transactionModel.AccountId));
                Assert.That(transaction.Reference, Is.EqualTo(transactionModel.Reference));
                Assert.That(transaction.CreatedOn, Is.EqualTo(transactionModel.CreatedOn));
                Assert.That(transaction.TransactionType, Is.EqualTo(transactionModel.TransactionType));
                Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transaction.Amount));
            });
        }

        [Test]
        public async Task CreateTransaction_ShouldAddNewTransaction_AndIncreaseAccountBalance()
        {
            //Arrange
            var transactionModel = new TransactionFormShortServiceModel()
            {
                Amount = 100,
                AccountId = this.Account1User1.Id,
                OwnerId = this.User1.Id,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "Not Initial Balance",
                TransactionType = TransactionType.Income
            };
            int transactionsCountBefore = this.Account1User1.Transactions.Count;
            decimal balanceBefore = this.Account1User1.Balance;

            //Act
            Guid id = await this.accountService.CreateTransactionAsync(transactionModel);
            Transaction? transaction = await this.transactionsRepo.FindAsync(id);

            //Assert
            Assert.That(transaction, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(this.Account1User1.Transactions, Has.Count.EqualTo(transactionsCountBefore + 1));
                Assert.That(transaction.Amount, Is.EqualTo(transactionModel.Amount));
                Assert.That(transaction.CategoryId, Is.EqualTo(transactionModel.CategoryId));
                Assert.That(transaction.AccountId, Is.EqualTo(transactionModel.AccountId));
                Assert.That(transaction.Reference, Is.EqualTo(transactionModel.Reference));
                Assert.That(transaction.CreatedOn, Is.EqualTo(transactionModel.CreatedOn));
                Assert.That(transaction.TransactionType, Is.EqualTo(transactionModel.TransactionType));
                Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore + transaction.Amount));
            });
        }

        [Test]
        public void CreateTransaction_ShouldThrowException_WhenAccountDoesNotExist()
        {
            //Arrange
            var inputFormModel = new TransactionFormShortServiceModel
            {
                Amount = 100,
                AccountId = Guid.NewGuid(),
                OwnerId = this.User1.Id,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "Not Initial Balance",
                TransactionType = TransactionType.Expense
            };

            //Act & Assert
            Assert.That(async () => await this.accountService.CreateTransactionAsync(inputFormModel),
            Throws.TypeOf<InvalidOperationException>());
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
                AccountTypeId = this.AccType1User1.Id,
                Balance = 0,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id,
                Transactions = new HashSet<Transaction>
                {
                    new Transaction
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = this.User1.Id,
                        Amount = 200,
                        CategoryId = this.CatInitialBalance.Id,
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
            await this.accountService.DeleteAccountAsync(accId, this.User1.Id, isUserAdmin: false, shouldDeleteTransactions: false);

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
                AccountTypeId = this.AccType1User1.Id,
                Balance = 0,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id,
                Transactions = new HashSet<Transaction>
                {
                    new Transaction
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = this.User1.Id,
                        Amount = 200,
                        CategoryId = this.CatInitialBalance.Id,
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
            await this.accountService.DeleteAccountAsync(accountId, this.User1.Id, isUserAdmin: false, shouldDeleteTransactions: true);

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
            Assert.That(async () => await this.accountService.DeleteAccountAsync(
                Guid.NewGuid(), this.User1.Id, isUserAdmin: false, shouldDeleteTransactions: true),
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
                AccountTypeId = this.AccType1User1.Id,
                CurrencyId = this.Curr1User1.Id,
                Balance = 0,
                Name = "For Delete",
                OwnerId = this.User1.Id
            });
            await this.accountsRepo.SaveChangesAsync();

            //Act & Assert
            Assert.That(async () => await this.accountService.DeleteAccountAsync(
                accId, this.User2.Id, isUserAdmin: false, shouldDeleteTransactions: true),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Can't delete someone else account."));
        }

        [Test]
        public async Task DeleteAccount_ShouldDeleteAccountAndTransactions_WhenUserIsAdmin()
        {
            //Arrange
            var accId = Guid.NewGuid();
            await this.accountsRepo.AddAsync(new Account
            {
                Id = accId,
                AccountTypeId = this.AccType1User1.Id,
                CurrencyId = this.Curr1User1.Id,
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
                        CategoryId = this.CatInitialBalance.Id,
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
            await this.accountService.DeleteAccountAsync(accId, this.User2.Id, isUserAdmin: true, shouldDeleteTransactions: true);

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
                AccountTypeId = this.AccType1User1.Id,
                CurrencyId = this.Curr1User1.Id,
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
                        CategoryId = this.CatInitialBalance.Id,
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
            await this.accountService.DeleteAccountAsync(accId, this.User2.Id, isUserAdmin: true, shouldDeleteTransactions: false);

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
                AccountId = this.Account1User1.Id,
                Amount = 123,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "TestTransaction",
                TransactionType = TransactionType.Income
            };
            await this.transactionsRepo.AddAsync(transaction);
            this.Account1User1.Balance += transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();

            decimal balanceBefore = this.Account1User1.Balance;
            int transactionsBefore = this.Account1User1.Transactions.Count;
            Transaction? transactionInDb = await this.transactionsRepo.FindAsync(transactionId);

            //Assert
            Assert.That(transactionInDb, Is.Not.Null);

            //Act
            decimal newBalance = await this.accountService.DeleteTransactionAsync(transactionId, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transactionInDb.Amount));
                Assert.That(this.Account1User1.Balance, Is.EqualTo(newBalance));
                Assert.That(this.Account1User1.Transactions, Has.Count.EqualTo(transactionsBefore - 1));
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
                AccountId = this.Account1User1.Id,
                Amount = 123,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "TestTransaction",
                TransactionType = TransactionType.Income
            };
            await this.transactionsRepo.AddAsync(transaction);
            this.Account1User1.Balance += transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();

            decimal balanceBefore = this.Account1User1.Balance;
            int transactionsBefore = this.Account1User1.Transactions.Count;
            Transaction? transactionInDb = await this.transactionsRepo.FindAsync(transactionId);

            //Assert
            Assert.That(transactionInDb, Is.Not.Null);

            //Act
            decimal newBalance = await this.accountService.DeleteTransactionAsync(transactionId, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transactionInDb.Amount));
                Assert.That(this.Account1User1.Balance, Is.EqualTo(newBalance));
                Assert.That(this.Account1User1.Transactions, Has.Count.EqualTo(transactionsBefore - 1));
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
                AccountId = this.Account1User1.Id,
                OwnerId = this.User1.Id,
                Amount = 123,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "TestTransaction",
                TransactionType = TransactionType.Expense
            };
            await this.transactionsRepo.AddAsync(transaction);
            this.Account1User1.Balance -= transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();

            decimal balanceBefore = this.Account1User1.Balance;
            int transactionsBefore = this.Account1User1.Transactions.Count;
            Transaction? transactionInDb = await this.transactionsRepo.FindAsync(transactionId);

            //Assert
            Assert.That(transactionInDb, Is.Not.Null);

            //Act
            decimal newBalance = await this.accountService.DeleteTransactionAsync(transactionId, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
                Assert.That(this.Account1User1.Balance, Is.EqualTo(newBalance));
                Assert.That(this.Account1User1.Transactions, Has.Count.EqualTo(transactionsBefore - 1));
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
                AccountId = this.Account1User1.Id,
                OwnerId = this.User1.Id,
                Amount = 123,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "TestTransaction",
                TransactionType = TransactionType.Expense
            };
            await this.transactionsRepo.AddAsync(transaction);
            this.Account1User1.Balance -= transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();

            decimal balanceBefore = this.Account1User1.Balance;
            int transactionsBefore = this.Account1User1.Transactions.Count;
            Transaction? transactionInDb = await this.transactionsRepo.FindAsync(transactionId);

            //Assert
            Assert.That(transactionInDb, Is.Not.Null);

            //Act
            decimal newBalance = await this.accountService.DeleteTransactionAsync(transactionId, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
                Assert.That(this.Account1User1.Balance, Is.EqualTo(newBalance));
                Assert.That(this.Account1User1.Transactions, Has.Count.EqualTo(transactionsBefore - 1));
                Assert.That(await this.transactionsRepo.FindAsync(transactionId), Is.Null);
            });
        }

        [Test]
        public void DeleteTransaction_ShouldThrowAnException_WithInvalidInput()
        {
            //Arrange

            //Act & Assert
            Assert.That(async () => await this.accountService.DeleteTransactionAsync(Guid.NewGuid(), this.User1.Id, isUserAdmin: false),
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
                AccountId = this.Account1User1.Id,
                CategoryId = this.Curr1User1.Id,
                Amount = 10,
                CreatedOn = DateTime.UtcNow,
                OwnerId = this.User1.Id,
                Reference = "For Delete",
                TransactionType = TransactionType.Expense
            };
            await this.transactionsRepo.AddAsync(transaction);
            await this.transactionsRepo.SaveChangesAsync();

            //Act & Assert
            Assert.That(async () => await this.accountService.DeleteTransactionAsync(id, this.User2.Id, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo("User is not transaction's owner"));
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
                AccountTypeId = this.AccType1User1.Id,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id
            };
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            var inputModel = new AccountFormShortServiceModel
            {
                Name = "New Name", // Change
                AccountTypeId = account.AccountTypeId,
                Balance = account.Balance,
                CurrencyId = account.CurrencyId,
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountService.EditAccountAsync(accId, inputModel);

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
                AccountTypeId = this.AccType1User1.Id,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id
            };
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            var inputModel = new AccountFormShortServiceModel
            {
                Name = account.Name,
                AccountTypeId = account.AccountTypeId,
                Balance = account.Balance,
                CurrencyId = this.Curr2User1.Id, // Change
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountService.EditAccountAsync(accId, inputModel);

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
            var accId = Guid.NewGuid();
            var account = new Account
            {
                Id = accId,
                Name = "For Edit 3",
                Balance = 10,
                AccountTypeId = this.AccType1User1.Id,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User2.Id
            };
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            var inputModel = new AccountFormShortServiceModel
            {
                Name = account.Name,
                AccountTypeId = this.AccType2User1.Id, // Change
                Balance = account.Balance,
                CurrencyId = account.CurrencyId,
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountService.EditAccountAsync(accId, inputModel);

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
                AccountTypeId = this.AccType1User1.Id,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id
            };
            var initialBalTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = 10,
                OwnerId = account.OwnerId,
                AccountId = account.Id,
                CategoryId = this.CatInitialBalance.Id,
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
                CategoryId = this.CatInitialBalance.Id,
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

            var inputModel = new AccountFormShortServiceModel
            {
                Name = account.Name,
                AccountTypeId = account.AccountTypeId,
                Balance = account.Balance + 100, // Change
                CurrencyId = account.CurrencyId,
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountService.EditAccountAsync(account.Id, inputModel);

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
                AccountTypeId = this.AccType1User1.Id,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id
            };
            var initialBalTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = 10,
                OwnerId = account.OwnerId,
                AccountId = account.Id,
                CategoryId = this.CatInitialBalance.Id,
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
                CategoryId = this.CatInitialBalance.Id,
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

            var inputModel = new AccountFormShortServiceModel
            {
                Name = account.Name,
                AccountTypeId = account.AccountTypeId,
                Balance = account.Balance - 100, // Change
                CurrencyId = account.CurrencyId,
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountService.EditAccountAsync(account.Id, inputModel);

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
                AccountTypeId = this.AccType1User1.Id,
                CurrencyId = this.Curr1User1.Id,
                OwnerId = this.User1.Id
            };
            await this.accountsRepo.AddAsync(account);
            await this.accountsRepo.SaveChangesAsync();

            var inputModel = new AccountFormShortServiceModel
            {
                Name = account.Name,
                AccountTypeId = account.AccountTypeId,
                Balance = account.Balance + 100, // Change
                CurrencyId = account.CurrencyId,
                OwnerId = account.OwnerId
            };

            //Act
            await this.accountService.EditAccountAsync(account.Id, inputModel);

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
                Assert.That(initialBalTransaction.CategoryId, Is.EqualTo(this.CatInitialBalance.Id));
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
            var inputModel = new AccountFormShortServiceModel
            {
                Name = this.Account2User1.Name, // Change
                AccountTypeId = this.Account1User1.AccountTypeId,
                Balance = this.Account1User1.Balance + 100,
                CurrencyId = this.Account1User1.CurrencyId,
                OwnerId = this.Account1User1.OwnerId
            };

            //Act & Assert
            Assert.That(async () => await this.accountService.EditAccountAsync(this.Account1User1.Id, inputModel),
            Throws.TypeOf<ArgumentException>().With.Message
                  .EqualTo($"The User already have Account with \"{this.Account2User1.Name}\" name."));
        }

        [Test]
        public async Task EditTransaction_ShouldEditTransactionAndChangeBalance_WhenTransactionTypeIsChanged()
        {
            //Arrange
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                OwnerId = this.User1.Id,
                AccountId = this.Account1User1.Id,
                Amount = 123,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "TransactionTypeChanged",
                TransactionType = TransactionType.Income
            };

            await this.transactionsRepo.AddAsync(transaction);
            this.Account1User1.Balance += transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();
            decimal balanceBefore = this.Account1User1.Balance;

            TransactionFormShortServiceModel transactionEditModel = await this.transactionsRepo.All()
                .Where(t => t.Id == transaction.Id)
                .Select(t => this.mapper.Map<TransactionFormShortServiceModel>(t))
                .FirstAsync();

            //Act
            transactionEditModel.TransactionType = TransactionType.Expense;
            await this.accountService.EditTransactionAsync(transaction.Id, transactionEditModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(transaction.TransactionType,
                    Is.EqualTo(transactionEditModel.TransactionType));

                Assert.That(this.Account1User1.Balance,
                    Is.EqualTo(balanceBefore - (transaction.Amount * 2)));
            });
        }

        [Test]
        public async Task EditTransaction_ShouldEditTransactionAndChangeBalanceOnTwoAccounts_WhenAccountIsChanged()
        {
            //Arrange
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                OwnerId = this.User1.Id,
                AccountId = this.Account2User1.Id,
                Amount = 123,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "AccountChanged",
                TransactionType = TransactionType.Income
            };

            await this.transactionsRepo.AddAsync(transaction);
            this.Account2User1.Balance += transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();
            decimal firstAccBalanceBefore = this.Account2User1.Balance;
            decimal secondAccBalanceBefore = this.Account1User1.Balance;

            //Act
            TransactionFormShortServiceModel editTransactionModel = await this.transactionsRepo.All()
                .Where(t => t.Id == transaction.Id)
                .Select(t => this.mapper.Map<TransactionFormShortServiceModel>(t))
                .FirstAsync();

            editTransactionModel.AccountId = this.Account1User1.Id;
            await this.accountService.EditTransactionAsync(transaction.Id, editTransactionModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(transaction.AccountId,
                    Is.EqualTo(this.Account1User1.Id));

                Assert.That(this.Account2User1.Balance,
                    Is.EqualTo(firstAccBalanceBefore - transaction.Amount));

                Assert.That(this.Account1User1.Balance,
                    Is.EqualTo(secondAccBalanceBefore + transaction.Amount));
            });
        }

        [Test]
        public async Task EditTransaction_ShouldEditTransaction_WhenPaymentReferenceIsChanged()
        {
            //Arrange
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                OwnerId = this.User1.Id,
                AccountId = this.Account1User1.Id,
                Amount = 123,
                CategoryId = this.CatInitialBalance.Id,
                CreatedOn = DateTime.UtcNow,
                Reference = "First Reference",
                TransactionType = TransactionType.Income
            };

            await this.transactionsRepo.AddAsync(transaction);
            this.Account1User1.Balance += transaction.Amount;
            await this.transactionsRepo.SaveChangesAsync();
            decimal balanceBefore = this.Account1User1.Balance;

            //Act
            TransactionFormShortServiceModel editTransactionModel = await this.transactionsRepo.All()
                .Where(t => t.Id == transaction.Id)
                .Select(t => this.mapper.Map<TransactionFormShortServiceModel>(t))
                .FirstAsync();
            editTransactionModel.Reference = "Second Reference";
            await this.accountService.EditTransactionAsync(transaction.Id, editTransactionModel);

            //Assert that only transaction reference is changed
            Assert.Multiple(() =>
            {

                Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore));
                Assert.That(transaction.Reference, Is.EqualTo(editTransactionModel.Reference));
                Assert.That(transaction.CategoryId, Is.EqualTo(editTransactionModel.CategoryId));
                Assert.That(transaction.AccountId, Is.EqualTo(editTransactionModel.AccountId));
                Assert.That(transaction.Amount, Is.EqualTo(editTransactionModel.Amount));
                Assert.That(transaction.OwnerId, Is.EqualTo(editTransactionModel.OwnerId));
                Assert.That(transaction.CreatedOn, Is.EqualTo(editTransactionModel.CreatedOn));
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
            AccountDetailsServiceModel actual = await this.accountService.GetAccountDetailsAsync(
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
            Assert.That(async () => await this.accountService.GetAccountDetailsAsync(id, startDate, endDate, this.User1.Id, isUserAdmin: false),
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
            UsersAccountsCardsServiceModel actual = await this.accountService.GetAccountsCardsDataAsync(1);

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
                await this.accountService.GetCashFlowByCurrenciesAsync();

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
            string actualName = await this.accountService
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
            Assert.That(async () => await this.accountService.GetAccountNameAsync(invalidId, this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<InvalidOperationException>());
        }

		[Test]
		public async Task GetAccountsCountAsync_ShouldReturnAccountsCount()
		{
			//Arrange
			int expectedCount = await this.accountsRepo.All().CountAsync(a => !a.IsDeleted);

			//Act
			int actualCount = await this.accountService.GetAccountsCountAsync();

			//Assert
			Assert.That(actualCount, Is.EqualTo(expectedCount));
		}

		[Test]
        public async Task GetAccountFormData_ShouldReturnCorrectData()
        {
            //Arrange

            //Act
            AccountFormServiceModel formData = await this.accountService.GetAccountFormDataAsync(this.Account1User1.Id, this.User1.Id, isUserAdmin: false);

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
            AccountFormServiceModel formData = await this.accountService.GetAccountFormDataAsync(this.Account1User1.Id, this.User2.Id, isUserAdmin: true);

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
            Assert.That(async () => await this.accountService.GetAccountFormDataAsync(invalidId, this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GetAccountFormData_ShouldThrowException_WhenUserIsNotOwner()
        {
            //Arrange

            //Act & Assert
            Assert.That(async () => await this.accountService.GetAccountFormDataAsync(this.Account1User1.Id, this.User2.Id, isUserAdmin: false),
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
                await this.accountService.GetTransactionFormDataAsync(this.Transaction2User1.Id);

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
                await this.accountService.GetTransactionFormDataAsync(this.Transaction1User1.Id);

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
            Assert.That(async () => await this.accountService.GetTransactionFormDataAsync(Guid.NewGuid()),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WithValidInput()
        {
            //Act
            TransactionDetailsServiceModel transactionFormModel =
                await this.accountService.GetTransactionDetailsAsync(this.Transaction1User1.Id, this.User1.Id, isUserAdmin: false);

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
            Guid ownerId = await this.accountService.GetOwnerIdAsync(this.Account1User1.Id);

            //Assert
            Assert.That(ownerId, Is.EqualTo(this.User1.Id));
        }

        [Test]
        public void GetOwnerId_ShouldThrowException_WhenAccountIdIsInvalid()
        {
            //Assert
            Assert.That(async () => await this.accountService.GetOwnerIdAsync(Guid.NewGuid()),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WhenUserIsAdmin()
        {
            //Act
            TransactionDetailsServiceModel transactionFormModel =
                await this.accountService.GetTransactionDetailsAsync(this.Transaction1User1.Id, this.User2.Id, isUserAdmin: true);

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
            Assert.That(async () => await this.accountService.GetTransactionDetailsAsync(invalidId, this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GetTransactionDetails_ShouldThrowException_WhenUserIsNotOwner()
        {
            //Arrange

            //Act & Assert
            Assert.That(async () => await this.accountService.GetTransactionDetailsAsync(this.Transaction1User1.Id, this.User2.Id, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo("User is not transaction's owner."));
        }

        [Test]
        public async Task GetAccountShortDetails_ShouldReturnCorrectData_WithValidInput()
        {
            //Act
            AccountDetailsShortServiceModel actualData = await this.accountService.GetAccountShortDetailsAsync(this.Account1User1.Id);

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
            Assert.That(async () => await this.accountService.GetAccountShortDetailsAsync(Guid.NewGuid()),
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
			IEnumerable<AccountCardServiceModel> actualAccounts = await this.accountService.GetUserAccountsAsync(this.User1.Id);

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
