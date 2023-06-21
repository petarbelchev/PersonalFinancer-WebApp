namespace PersonalFinancer.Services.User
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Enums;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.MemoryCacheService;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Services.User.Models;
    using static PersonalFinancer.Data.Constants;
    using static PersonalFinancer.Services.Constants;

    public class UsersService : IUsersService
    {
        private readonly IEfRepository<ApplicationUser> usersRepo;
        private readonly IMapper mapper;
        private readonly IMemoryCacheService<Category> categoriesCacheService;
        private readonly IMemoryCacheService<Currency> currenciesCacheService;
        private readonly IMemoryCacheService<AccountType> accountTypesCacheService;
        private readonly IMemoryCacheService<Account> accountsCacheService;

		public UsersService(
            IEfRepository<ApplicationUser> usersRepo,
            IMapper mapper,
            IMemoryCacheService<Category> categoriesCache,
			IMemoryCacheService<Currency> currenciesCache,
			IMemoryCacheService<AccountType> accountTypesCache,
			IMemoryCacheService<Account> accountsCache)
        {
            this.usersRepo = usersRepo;
            this.mapper = mapper;
            this.categoriesCacheService = categoriesCache;
            this.currenciesCacheService = currenciesCache;
            this.accountTypesCacheService = accountTypesCache;
            this.accountsCacheService = accountsCache;
        }

		public async Task<UsersServiceModel> GetAllUsersAsync(int page)
		{
			return new UsersServiceModel
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
		}

		public async Task<IEnumerable<AccountServiceModel>> GetUserAccountsDropdownData(Guid userId)
		{
			return await this.accountsCacheService
				.GetValues<AccountServiceModel>(AccountConstants.AccountCacheKeyValue, userId);
		}

		public async Task<IEnumerable<AccountTypeServiceModel>> GetUserAccountTypesDropdownData(Guid userId)
		{
			return await this.accountTypesCacheService
				.GetValues<AccountTypeServiceModel>(AccountTypeConstants.AccTypeCacheKeyValue, userId);
		}

		public async Task<IEnumerable<CategoryServiceModel>> GetUserCategoriesDropdownData(Guid userId)
		{
			return await this.categoriesCacheService
				.GetValues<CategoryServiceModel>(CategoryConstants.CategoryCacheKeyValue, userId);
		}

		public async Task<IEnumerable<CurrencyServiceModel>> GetUserCurrenciesDropdownData(Guid userId)
		{
			return await this.currenciesCacheService
				.GetValues<CurrencyServiceModel>(CurrencyConstants.CurrencyCacheKeyValue, userId);
		}

		public async Task<UserDashboardServiceModel> GetUserDashboardDataAsync(
			Guid userId, DateTime startDate, DateTime endDate)
		{
			DateTime startDateUtc = startDate.ToUniversalTime();
			DateTime endDateUtc = endDate.ToUniversalTime();

			return await this.usersRepo.All()
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
							Incomes = t
								.Where(t => t.TransactionType == TransactionType.Income)
								.Sum(t => t.Amount),
							Expenses = t
								.Where(t => t.TransactionType == TransactionType.Expense)
								.Sum(t => t.Amount),
							ExpensesByCategories = t
								.Where(t => t.TransactionType == TransactionType.Expense)
								.GroupBy(t => t.Category.Name)
								.Select(t => new CategoryExpensesServiceModel
								{
									CategoryName = t.Key,
									ExpensesAmount = t
										.Where(t => t.TransactionType == TransactionType.Expense)
										.Sum(t => t.Amount)
								})
						})
						.OrderBy(c => c.Name)
						.ToList()
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<UserDetailsServiceModel> UserDetailsAsync(Guid userId)
		{
			return await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsServiceModel>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> UserFullNameAsync(Guid userId)
        {
			return await this.usersRepo.All()
                .Where(u => u.Id == userId)
                .Select(u => $"{u.FirstName} {u.LastName}")
                .FirstAsync();
        }

        public async Task<int> UsersCountAsync()
            => await this.usersRepo.All().CountAsync();
	}
}