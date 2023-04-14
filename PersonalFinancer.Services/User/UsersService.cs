namespace PersonalFinancer.Services.User
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;
	using Data;
	using Data.Enums;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using Services.Shared.Models;
	using Services.User.Models;
	using static Data.Constants;

	public class UsersService : IUsersService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public UsersService(
			PersonalFinancerDbContext data,
			IMapper mapper,
			IMemoryCache memoryCache)
		{
			this.data = data;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> FullName(string userId)
		{
			string fullName = await data.Users
				.Where(u => u.Id == userId)
				.Select(u => $"{u.FirstName} {u.LastName}")
				.FirstAsync();

			return fullName;
		}

		public async Task<UsersServiceModel> GetAllUsers(int page)
		{
			var users = new UsersServiceModel
			{
				Users = await data.Users
					.OrderBy(u => u.FirstName)
					.ThenBy(u => u.LastName)
					.Skip(PaginationConstants.UsersPerPage * (page - 1))
					.Take(PaginationConstants.UsersPerPage)
					.ProjectTo<UserServiceModel>(mapper.ConfigurationProvider)
					.ToListAsync(),
				TotalUsersCount = data.Users.Count()
			};

			return users;
		}

		public async Task<UserAccountsAndCategoriesServiceModel> GetUserAccountsAndCategories(string userId)
		{
			if (!memoryCache.TryGetValue(CategoryConstants.CategoryCacheKeyValue + userId,
				out CategoryServiceModel[] categories))
			{
				categories = await data.Categories
					.Where(c => c.OwnerId == userId && !c.IsDeleted)
					.Select(c => mapper.Map<CategoryServiceModel>(c))
					.ToArrayAsync();

				memoryCache.Set(CategoryConstants.CategoryCacheKeyValue + userId, categories, TimeSpan.FromDays(3));
			}

			if (!memoryCache.TryGetValue(AccountConstants.AccountCacheKeyValue + userId,
				out AccountServiceModel[] accounts))
			{
				accounts = await data.Accounts
					.Where(a => a.OwnerId == userId && !a.IsDeleted)
					.Select(a => mapper.Map<AccountServiceModel>(a))
					.ToArrayAsync();
				
				memoryCache.Set(AccountConstants.AccountCacheKeyValue + userId, accounts, TimeSpan.FromDays(3));
			}

			var userData = new UserAccountsAndCategoriesServiceModel
			{
				OwnerId = userId,
				UserAccounts = accounts,
				UserCategories = categories
			};

			return userData;
		}

		public async Task<UserAccountTypesAndCurrenciesServiceModel> GetUserAccountTypesAndCurrencies(string userId)
		{
			if (!memoryCache.TryGetValue(AccountTypeConstants.AccTypeCacheKeyValue + userId,
				out AccountTypeServiceModel[] accTypes))
			{
				accTypes = await data.AccountTypes
					.Where(at => at.OwnerId == userId && !at.IsDeleted)
					.Select(at => mapper.Map<AccountTypeServiceModel>(at))
					.ToArrayAsync();

				memoryCache.Set(AccountTypeConstants.AccTypeCacheKeyValue + userId, accTypes, TimeSpan.FromDays(3));
			}

			if (!memoryCache.TryGetValue(CurrencyConstants.CurrencyCacheKeyValue + userId,
				out CurrencyServiceModel[] currencies))
			{
				currencies = await data.Currencies
					.Where(c => c.OwnerId == userId && !c.IsDeleted)
					.Select(c => mapper.Map<CurrencyServiceModel>(c))
					.ToArrayAsync();
				
				memoryCache.Set(CurrencyConstants.CurrencyCacheKeyValue + userId, currencies, TimeSpan.FromDays(3));
			}

			var userData = new UserAccountTypesAndCurrenciesServiceModel
			{
				AccountTypes = accTypes,
				Currencies = currencies
			};

			return userData;
		}

		public async Task<IEnumerable<AccountCardServiceModel>> GetUserAccounts(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => mapper.Map<AccountCardServiceModel>(a))
				.ToArrayAsync();
		}

		public int GetUsersAccountsCount()
		{
			int accountsCount = data.Accounts.Count(a => !a.IsDeleted);

			return accountsCount;
		}

		public async Task<TransactionsServiceModel> GetUserTransactions(string userId, DateTime startDate, DateTime endDate, int page = 1)
		{
			TransactionsServiceModel userTransactions = await data.Users
				.Where(u => u.Id == userId)
				.Select(u => new TransactionsServiceModel
				{
					StartDate = startDate,
					EndDate = endDate,
					TotalTransactionsCount = u.Transactions
						.Count(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate),
					Transactions = u.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
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
							CreatedOn = t.CreatedOn,
							Refference = t.Refference,
							TransactionType = t.TransactionType.ToString()
						})
				})
				.FirstAsync();

			return userTransactions;
		}

		public async Task<UserDashboardServiceModel> GetUserDashboardData(string userId, DateTime startDate, DateTime endDate)
		{
			var dto = await data.Users
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
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.OrderByDescending(t => t.CreatedOn)
						.Take(5)
						.Select(t => new TransactionTableServiceModel
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = t.Account.Currency.Name,
							CreatedOn = t.CreatedOn,
							TransactionType = t.TransactionType.ToString(),
							Refference = t.Refference,
							CategoryName = t.Category.Name
						}),
					CurrenciesCashFlow = u.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.GroupBy(t => t.Account.Currency.Name)
						.Select(t => new CurrencyCashFlowServiceModel
						{
							Name = t.Key,
							Incomes = t.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
							Expenses = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
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
		public async Task<UserDetailsServiceModel> UserDetails(string userId)
		{
			var result = await data.Users
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsServiceModel>(mapper.ConfigurationProvider)
				.FirstAsync();

			return result;
		}

		public int UsersCount() => data.Users.Count();
	}
}