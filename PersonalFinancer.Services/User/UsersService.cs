namespace PersonalFinancer.Services.User
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Enums;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Services.User.Models;
    using static PersonalFinancer.Data.Constants;
    using static PersonalFinancer.Services.Infrastructure.Constants;

    public class UsersService : IUsersService
    {
        private readonly IEfRepository<ApplicationUser> usersRepo;
        private readonly IEfRepository<Category> categoriesRepo;
        private readonly IEfRepository<Account> accountsRepo;
        private readonly IEfRepository<AccountType> accountTypesRepo;
        private readonly IEfRepository<Currency> currenciesRepo;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public UsersService(
            IEfRepository<ApplicationUser> usersRepo,
            IEfRepository<Category> categoriesRepo,
            IEfRepository<Account> accountsRepo,
            IEfRepository<AccountType> accountTypesRepo,
            IEfRepository<Currency> currenciesRepo,
            IMapper mapper,
            IMemoryCache memoryCache)
        {
            this.usersRepo = usersRepo;
            this.categoriesRepo = categoriesRepo;
            this.accountsRepo = accountsRepo;
            this.accountTypesRepo = accountTypesRepo;
            this.currenciesRepo = currenciesRepo;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// Throws InvalidOperationException if User does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> UserFullName(Guid userId)
        {
            string fullName = await this.usersRepo.All()
                .Where(u => u.Id == userId)
                .Select(u => $"{u.FirstName} {u.LastName}")
                .FirstAsync();

            return fullName;
        }

        public async Task<UsersServiceModel> GetAllUsers(int page)
        {
            var users = new UsersServiceModel
            {
                Users = await this.usersRepo.All()
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Skip(PaginationConstants.UsersPerPage * (page - 1))
                    .Take(PaginationConstants.UsersPerPage)
                    .ProjectTo<UserServiceModel>(this.mapper.ConfigurationProvider)
                    .ToListAsync(),
                TotalUsersCount = await this.usersRepo.All().CountAsync()
            };

            return users;
        }

        public async Task<UserAccountsAndCategoriesServiceModel> GetUserAccountsAndCategories(Guid? userId)
        {
            if (!this.memoryCache.TryGetValue(CategoryConstants.CategoryCacheKeyValue + userId,
                out CategoryServiceModel[] categories))
            {
                categories = await this.categoriesRepo.All()
                    .Where(c => c.OwnerId == userId && !c.IsDeleted)
                    .OrderBy(c => c.Name)
                    .Select(c => this.mapper.Map<CategoryServiceModel>(c))
                    .ToArrayAsync();

                _ = this.memoryCache.Set(CategoryConstants.CategoryCacheKeyValue + userId, categories, TimeSpan.FromDays(3));
            }

            if (!this.memoryCache.TryGetValue(AccountConstants.AccountCacheKeyValue + userId,
                out AccountServiceModel[] accounts))
            {
                accounts = await this.accountsRepo.All()
                    .Where(a => a.OwnerId == userId && !a.IsDeleted)
                    .OrderBy(a => a.Name)
                    .Select(a => this.mapper.Map<AccountServiceModel>(a))
                    .ToArrayAsync();

                _ = this.memoryCache.Set(AccountConstants.AccountCacheKeyValue + userId, accounts, TimeSpan.FromDays(3));
            }

            var userData = new UserAccountsAndCategoriesServiceModel
            {
                OwnerId = userId ?? Guid.Empty,
                UserAccounts = accounts,
                UserCategories = categories
            };

            return userData;
        }

        public async Task<UserAccountTypesAndCurrenciesServiceModel> GetUserAccountTypesAndCurrencies(Guid? userId)
        {
            if (!this.memoryCache.TryGetValue(AccountTypeConstants.AccTypeCacheKeyValue + userId,
                out AccountTypeServiceModel[] accTypes))
            {
                accTypes = await this.accountTypesRepo.All()
                    .Where(at => at.OwnerId == userId && !at.IsDeleted)
                    .OrderBy(at => at.Name)
                    .Select(at => this.mapper.Map<AccountTypeServiceModel>(at))
                    .ToArrayAsync();

                _ = this.memoryCache.Set(AccountTypeConstants.AccTypeCacheKeyValue + userId, accTypes, TimeSpan.FromDays(3));
            }

            if (!this.memoryCache.TryGetValue(CurrencyConstants.CurrencyCacheKeyValue + userId,
                out CurrencyServiceModel[] currencies))
            {
                currencies = await this.currenciesRepo.All()
                    .Where(c => c.OwnerId == userId && !c.IsDeleted)
                    .OrderBy(c => c.Name)
                    .Select(c => this.mapper.Map<CurrencyServiceModel>(c))
                    .ToArrayAsync();

                _ = this.memoryCache.Set(CurrencyConstants.CurrencyCacheKeyValue + userId, currencies, TimeSpan.FromDays(3));
            }

            var userData = new UserAccountTypesAndCurrenciesServiceModel
            {
                AccountTypes = accTypes,
                Currencies = currencies
            };

            return userData;
        }

        public async Task<IEnumerable<AccountCardServiceModel>> GetUserAccounts(Guid userId)
        {
            return await this.accountsRepo.All()
                .Where(a => a.OwnerId == userId && !a.IsDeleted)
                .OrderBy(a => a.Name)
                .Select(a => this.mapper.Map<AccountCardServiceModel>(a))
                .ToArrayAsync();
        }

        public async Task<int> GetUsersAccountsCount()
            => await this.accountsRepo.All().CountAsync(a => !a.IsDeleted);

        public async Task<TransactionsServiceModel> GetUserTransactions(
            Guid? userId, DateTime startDate, DateTime endDate, int page = 1)
        {
            DateTime startDateUtc = startDate.ToUniversalTime();
            DateTime endDateUtc = endDate.ToUniversalTime();

            TransactionsServiceModel userTransactions = await this.usersRepo.All()
                .Where(u => u.Id == userId)
                .Select(u => new TransactionsServiceModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalTransactionsCount = u.Transactions
                        .Count(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc),
                    Transactions = u.Transactions
                        .Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
                        .OrderByDescending(t => t.CreatedOn)
                        .Skip(PaginationConstants.TransactionsPerPage * (page - 1))
                        .Take(PaginationConstants.TransactionsPerPage)
                        .Select(t => new TransactionTableServiceModel
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            AccountCurrencyName = t.Account.Currency.Name,
                            CategoryName = t.Category.Name + (t.Category.IsDeleted ?
                                " (Deleted)"
                                : string.Empty),
                            CreatedOn = t.CreatedOn.ToLocalTime(),
                            Reference = t.Reference,
                            TransactionType = t.TransactionType.ToString()
                        })
                })
                .FirstAsync();

            return userTransactions;
        }

        public async Task<UserDashboardServiceModel> GetUserDashboardData(
            Guid userId, DateTime startDate, DateTime endDate)
        {
            DateTime startDateUtc = startDate.ToUniversalTime();
            DateTime endDateUtc = endDate.ToUniversalTime();

            UserDashboardServiceModel dto = await this.usersRepo.All()
                .Where(u => u.Id == userId)
                .Select(u => new UserDashboardServiceModel
                {
                    Accounts = u.Accounts.Where(a => !a.IsDeleted)
                        .OrderBy(a => a.Name)
                        .Select(a => new AccountCardServiceModel
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Balance = a.Balance,
                            CurrencyName = a.Currency.Name
                        }),
                    LastTransactions = u.Transactions
                        .Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
                        .OrderByDescending(t => t.CreatedOn)
                        .Take(5)
                        .Select(t => new TransactionTableServiceModel
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            AccountCurrencyName = t.Account.Currency.Name,
                            CreatedOn = t.CreatedOn.ToLocalTime(),
                            TransactionType = t.TransactionType.ToString(),
                            Reference = t.Reference,
                            CategoryName = t.Category.Name
                        }),
                    CurrenciesCashFlow = u.Transactions
                        .Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
                        .GroupBy(t => t.Account.Currency.Name)
                        .Select(t => new CurrencyCashFlowServiceModel
                        {
                            Name = t.Key,
                            Incomes = t.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
                            Expenses = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount),
                            ExpensesByCategories = t
                                .Where(t => t.TransactionType == TransactionType.Expense)
                                .GroupBy(t => t.Category.Name)
                                .Select(t => new CategoryExpensesServiceModel
                                {
                                    CategoryName = t.Key,
                                    ExpensesAmount = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
                                })
                        })
                        .OrderBy(c => c.Name)
                        .ToList()
                })
                .FirstAsync();

            return dto;
        }

        /// <summary>
        /// Throws InvalidOperationException if User does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<UserDetailsServiceModel> UserDetails(Guid? userId)
        {
            UserDetailsServiceModel result = await this.usersRepo.All()
                .Where(u => u.Id == userId)
                .ProjectTo<UserDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstAsync();

            return result;
        }

        public async Task<int> UsersCount()
            => await this.usersRepo.All().CountAsync();
    }
}