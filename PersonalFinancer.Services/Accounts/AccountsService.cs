namespace PersonalFinancer.Services.Accounts
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Enums;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Shared.Models;
    using static PersonalFinancer.Data.Constants;
    using static PersonalFinancer.Services.Infrastructure.Constants;

    public class AccountsService : IAccountsService
    {
        private readonly IEfRepository<Account> accountsRepo;
        private readonly IEfRepository<Transaction> transactionsRepo;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public AccountsService(
            IEfRepository<Account> accountRepository,
            IEfRepository<Transaction> transactionRepository,
            IMapper mapper,
            IMemoryCache memoryCache)
        {
            this.accountsRepo = accountRepository;
            this.transactionsRepo = transactionRepository;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        private static void ChangeBalance(Account account, decimal amount, TransactionType transactionType)
        {
            if (transactionType == TransactionType.Income)
                account.Balance += amount;
            else if (transactionType == TransactionType.Expense)
                account.Balance -= amount;
        }

        /// <summary>
        /// Throws ArgumentException when User already have Account with the given name.
        /// </summary>
        /// <returns>New Account Id.</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Guid> CreateAccount(AccountFormShortServiceModel model)
        {
            if (this.IsNameExists(model.Name, model.OwnerId))
                throw new ArgumentException($"The User already have Account with \"{model.Name}\" name.");

            Account newAccount = this.mapper.Map<Account>(model);
            newAccount.Id = Guid.NewGuid();

            if (newAccount.Balance != 0)
            {
                newAccount.Transactions.Add(new Transaction()
                {
                    Id = Guid.NewGuid(),
                    OwnerId = newAccount.OwnerId,
                    CategoryId = Guid.Parse(CategoryConstants.InitialBalanceCategoryId),
                    Amount = newAccount.Balance,
                    CreatedOn = DateTime.UtcNow,
                    TransactionType = TransactionType.Income,
                    Refference = CategoryConstants.CategoryInitialBalanceName,
                    IsInitialBalance = true
                });
            }

            await this.accountsRepo.AddAsync(newAccount);
            _ = await this.accountsRepo.SaveChangesAsync();

            this.memoryCache.Remove(AccountConstants.AccountCacheKeyValue + model.OwnerId);

            return newAccount.Id;
        }

        /// <summary>
        /// Throws InvalidOperationException if Account does not exist.
        /// </summary>
        /// <returns>New transaction Id.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Guid> CreateTransaction(TransactionFormShortServiceModel model)
        {
            Account account = await this.accountsRepo.All()
                .FirstAsync(a => a.Id == model.AccountId);

            model.CreatedOn = model.CreatedOn.ToUniversalTime();

            Transaction newTransaction = this.mapper.Map<Transaction>(model);
            newTransaction.Id = Guid.NewGuid();

            await transactionsRepo.AddAsync(newTransaction);

            if (model.TransactionType == TransactionType.Income)
                account.Balance += newTransaction.Amount;
            else if (model.TransactionType == TransactionType.Expense)
                account.Balance -= newTransaction.Amount;

            _ = await this.accountsRepo.SaveChangesAsync();

            return newTransaction.Id;
        }

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task DeleteAccount(
            Guid? accountId, Guid userId, bool isUserAdmin, bool shouldDeleteTransactions = false)
        {
            Account account = await this.accountsRepo.All()
                .FirstAsync(a => a.Id == accountId && !a.IsDeleted);

            if (!isUserAdmin && account.OwnerId != userId)
                throw new ArgumentException("Can't delete someone else account.");

            if (shouldDeleteTransactions)
                this.accountsRepo.Remove(account);
            else
                account.IsDeleted = true;

            _ = await this.accountsRepo.SaveChangesAsync();

            this.memoryCache.Remove(AccountConstants.AccountCacheKeyValue + userId);
        }

        /// <summary>
        /// Throws InvalidOperationException when Transaction does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<decimal> DeleteTransaction(Guid? transactionId, Guid userId, bool isUserAdmin)
        {
            Transaction transaction = await this.transactionsRepo.All()
               .Include(t => t.Account)
               .FirstAsync(t => t.Id == transactionId);

            if (!isUserAdmin && transaction.OwnerId != userId)
                throw new ArgumentException("User is not transaction's owner");

            this.transactionsRepo.Remove(transaction);

            transaction.TransactionType = transaction.TransactionType == TransactionType.Income ? TransactionType.Expense : TransactionType.Income;

            ChangeBalance(transaction.Account, transaction.Amount, transaction.TransactionType);

            _ = await this.transactionsRepo.SaveChangesAsync();

            return transaction.Account.Balance;
        }

        /// <summary>
        /// Throws InvalidOperationException when Account does now exist,
        /// and ArgumentException when User already have Account with given name.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task EditAccount(Guid? accountId, AccountFormShortServiceModel model)
        {
            Account account = await this.accountsRepo.All()
                .FirstAsync(a => a.Id == accountId);

            if (account.Name != model.Name && this.IsNameExists(model.Name, model.OwnerId))
                throw new ArgumentException($"The User already have Account with \"{model.Name}\" name.");

            account.Name = model.Name.Trim();
            account.CurrencyId = model.CurrencyId;
            account.AccountTypeId = model.AccountTypeId;

            if (account.Balance != model.Balance)
            {
                decimal amountOfChange = model.Balance - account.Balance;
                account.Balance = model.Balance;

                Transaction? transaction = await this.transactionsRepo.All()
                    .FirstOrDefaultAsync(t => t.AccountId == account.Id && t.IsInitialBalance);

                if (transaction == null)
                {
                    var initialBalance = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = account.OwnerId,
                        Amount = amountOfChange,
                        CategoryId = Guid.Parse(CategoryConstants.InitialBalanceCategoryId),
                        CreatedOn = DateTime.UtcNow,
                        Refference = CategoryConstants.CategoryInitialBalanceName,
                        TransactionType = amountOfChange < 0 ? TransactionType.Expense : TransactionType.Income,
                        IsInitialBalance = true,
                        AccountId = account.Id
                    };

                    await transactionsRepo.AddAsync(initialBalance);
                }
                else
                {
                    transaction.Amount += amountOfChange;

                    if (transaction.Amount < 0)
                        transaction.TransactionType = TransactionType.Expense;
                }
            }

            _ = await this.accountsRepo.SaveChangesAsync();
        }

        /// <summary>
        /// Throws InvalidOperationException when Transaction or Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task EditTransaction(Guid id, TransactionFormShortServiceModel model)
        {
            Transaction transactionInDb = await this.transactionsRepo.All()
                .Include(t => t.Account)
                .FirstAsync(t => t.Id == id);

            if (model.AccountId != transactionInDb.AccountId
                || model.TransactionType != transactionInDb.TransactionType
                || model.Amount != transactionInDb.Amount)
            {
                TransactionType opositeTransactionType = TransactionType.Income;

                if (transactionInDb.TransactionType == TransactionType.Income)
                    opositeTransactionType = TransactionType.Expense;

                ChangeBalance(transactionInDb.Account, transactionInDb.Amount, opositeTransactionType);

                if (model.AccountId != transactionInDb.AccountId)
                {
                    Account newAccount = await this.accountsRepo.All().FirstAsync(a => a.Id == model.AccountId);
                    transactionInDb.Account = newAccount;
                }

                ChangeBalance(transactionInDb.Account, model.Amount, model.TransactionType);
            }

            transactionInDb.Refference = model.Refference.Trim();
            transactionInDb.AccountId = model.AccountId;
            transactionInDb.CategoryId = model.CategoryId;
            transactionInDb.Amount = model.Amount;
            transactionInDb.CreatedOn = model.CreatedOn.ToUniversalTime();
            transactionInDb.TransactionType = model.TransactionType;

            _ = await this.transactionsRepo.SaveChangesAsync();
        }

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist
        /// or User is not owner or Administrator.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<AccountDetailsServiceModel> GetAccountDetails(
            Guid? id, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin)
        {
            DateTime startDateUtc = startDate.ToUniversalTime();
            DateTime endDateUtc = endDate.ToUniversalTime();

            return await this.accountsRepo.All()
                .Where(a => a.Id == id && !a.IsDeleted && (isUserAdmin || a.OwnerId == userId))
                .Select(a => new AccountDetailsServiceModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    OwnerId = a.OwnerId,
                    Balance = a.Balance,
                    CurrencyName = a.Currency.Name,
                    AccountTypeName = a.AccountType.Name,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalAccountTransactions = a.Transactions
                        .Count(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc),
                    Transactions = a.Transactions
                        .Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
                        .OrderByDescending(t => t.CreatedOn)
                        .Take(PaginationConstants.TransactionsPerPage)
                        .Select(t => new TransactionTableServiceModel
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            AccountCurrencyName = a.Currency.Name,
                            CreatedOn = t.CreatedOn.ToLocalTime(),
                            CategoryName = t.Category.Name + (t.Category.IsDeleted ?
                                " (Deleted)"
                                : string.Empty),
                            TransactionType = t.TransactionType.ToString(),
                            Refference = t.Refference
                        })
                })
                .FirstAsync();
        }

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist 
        /// or User is not owner or Administrator.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<AccountFormServiceModel> GetAccountFormData(
            Guid? accountId, Guid userId, bool isUserAdmin)
        {
            return await this.accountsRepo.All()
                .Where(a => a.Id == accountId && (isUserAdmin || a.OwnerId == userId))
                .ProjectTo<AccountFormServiceModel>(this.mapper.ConfigurationProvider)
                .FirstAsync();
        }

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<TransactionsServiceModel> GetAccountTransactions(
            Guid? id, DateTime startDate, DateTime endDate, int page)
        {
            DateTime startDateUtc = startDate.ToUniversalTime();
            DateTime endDateUtc = endDate.ToUniversalTime();

            TransactionsServiceModel accountTransactions = await this.accountsRepo.All()
                .Where(a => a.Id == id && !a.IsDeleted)
                .Select(a => new TransactionsServiceModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Transactions = a.Transactions
                        .Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
                        .OrderByDescending(t => t.CreatedOn)
                        .Skip(PaginationConstants.TransactionsPerPage * (page - 1))
                        .Take(PaginationConstants.TransactionsPerPage)
                        .Select(t => new TransactionTableServiceModel
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            AccountCurrencyName = a.Currency.Name,
                            CreatedOn = t.CreatedOn.ToLocalTime(),
                            CategoryName = t.Category.Name + (t.Category.IsDeleted ?
                                " (Deleted)"
                                : string.Empty),
                            TransactionType = t.TransactionType.ToString(),
                            Refference = t.Refference
                        }),
                    TotalTransactionsCount = a.Transactions
                        .Count(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc),
                })
                .FirstAsync();

            return accountTransactions;
        }

        public async Task<UsersAccountsCardsServiceModel> GetAccountsCardsData(int page)
        {
            var outputModel = new UsersAccountsCardsServiceModel
            {
                Accounts = await this.accountsRepo.All()
                    .Where(a => !a.IsDeleted)
                    .OrderBy(a => a.Name)
                    .Skip(PaginationConstants.AccountsPerPage * (page - 1))
                    .Take(PaginationConstants.AccountsPerPage)
                    .ProjectTo<AccountCardServiceModel>(this.mapper.ConfigurationProvider)
                    .ToArrayAsync(),
                TotalUsersAccountsCount = await this.accountsRepo.All().CountAsync(a => !a.IsDeleted)
            };

            return outputModel;
        }

        public async Task<IEnumerable<CurrencyCashFlowServiceModel>> GetCurrenciesCashFlow()
        {
            return await this.transactionsRepo.All()
                .GroupBy(t => t.Account.Currency.Name)
                .Select(t => new CurrencyCashFlowServiceModel
                {
                    Name = t.Key,
                    Incomes = t.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
                    Expenses = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
                })
                .OrderBy(c => c.Name)
                .ToArrayAsync();
        }

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist
        /// or User is not owner or Administrator.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> GetAccountName(Guid? accountId, Guid userId, bool isUserAdmin)
        {
            string name = await this.accountsRepo.All()
                .Where(a => a.Id == accountId && (isUserAdmin || a.OwnerId == userId))
                .Select(a => a.Name)
                .FirstAsync();

            return name;
        }

        /// <summary>
        /// Throws InvalidOperationException when Transaction does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<TransactionFormServiceModel> GetTransactionFormData(Guid transactionId)
        {
            return await this.transactionsRepo.All()
                .Where(t => t.Id == transactionId)
                .Select(t => new TransactionFormServiceModel
                {
                    OwnerId = t.OwnerId,
                    AccountId = t.AccountId,
                    CategoryId = t.CategoryId,
                    Amount = t.Amount,
                    CreatedOn = t.CreatedOn.ToLocalTime(),
                    TransactionType = t.TransactionType,
                    Refference = t.Refference,
                    IsInitialBalance = t.IsInitialBalance,
                    UserAccounts = t.Account.IsDeleted || t.IsInitialBalance ?
                        new List<AccountServiceModel>()
                        {
                            new AccountServiceModel { Id = t.AccountId, Name = t.Account.Name }
                        }
                        : t.Owner.Accounts.Where(a => !a.IsDeleted)
                            .OrderBy(a => a.Name)
                            .Select(a => this.mapper.Map<AccountServiceModel>(a)),
                    UserCategories = t.IsInitialBalance ?
                        new List<CategoryServiceModel>()
                        {
                            new CategoryServiceModel { Id = t.CategoryId, Name = t.Category.Name }
                        }
                        : t.Owner.Categories.Where(c => !c.IsDeleted)
                            .OrderBy(c => c.Name)
                            .Select(c => this.mapper.Map<CategoryServiceModel>(c))
                })
                .FirstAsync();
        }

        /// <summary>
        /// Throws InvalidOperationException if Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Guid> GetOwnerId(Guid? accountId)
        {
            Guid ownerId = await this.accountsRepo.All()
                .Where(a => a.Id == accountId)
                .Select(a => a.OwnerId)
                .FirstAsync();

            return ownerId;
        }

        /// <summary>
        /// Throws InvalidOperationException when Transaction does not exist
        /// and ArgumentException when the User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<TransactionDetailsServiceModel> GetTransactionDetails(
            Guid transactionId, Guid ownerId, bool isUserAdmin)
        {
            TransactionDetailsServiceModel? transaction = await this.transactionsRepo.All()
                .Where(t => t.Id == transactionId)
                .ProjectTo<TransactionDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (transaction == null)
                throw new InvalidOperationException("Transaction does not exist.");

            if (!isUserAdmin && transaction.OwnerId != ownerId)
                throw new ArgumentException("User is not transaction's owner.");

            transaction.CreatedOn = transaction.CreatedOn.ToLocalTime();

            return transaction;
        }

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> IsAccountDeleted(Guid accountId)
        {
            bool isDeleted = await this.accountsRepo.All()
                .Where(a => a.Id == accountId)
                .Select(a => a.IsDeleted)
                .FirstAsync();

            return isDeleted;
        }

        /// <summary>
        /// Throws ArgumentNullException when Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> IsAccountOwner(Guid userId, Guid accountId)
        {
            Guid ownerId = await this.accountsRepo.All()
                .Where(a => a.Id == accountId)
                .Select(a => a.OwnerId)
                .FirstAsync();

            return ownerId == userId;
        }

        private bool IsNameExists(string name, Guid userId)
        {
            bool isExist = this.accountsRepo.All()
                .Any(a => a.OwnerId == userId && a.Name == name);

            return isExist;
        }

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<AccountDetailsShortServiceModel> GetAccountShortDetails(Guid? accountId)
        {
            return await this.accountsRepo.All()
                .Where(a => a.Id == accountId)
                .ProjectTo<AccountDetailsShortServiceModel>(this.mapper.ConfigurationProvider)
                .FirstAsync();
        }
    }
}