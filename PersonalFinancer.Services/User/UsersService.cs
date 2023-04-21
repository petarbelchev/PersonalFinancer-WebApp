﻿namespace PersonalFinancer.Services.User
{	
	using AutoMapper;
	using AutoMapper.QueryableExtensions;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;

	using Data.Enums;
	using Data.Models;
	using static Data.Constants;
	
	using Services.Infrastructure;
	using Services.Shared.Models;
	using Services.User.Models;

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
		public async Task<string> FullName(string userId)
		{
			string fullName = await usersRepo.All()
				.Where(u => u.Id == userId)
				.Select(u => $"{u.FirstName} {u.LastName}")
				.FirstAsync();

			return fullName;
		}

		public async Task<UsersServiceModel> GetAllUsers(int page)
		{
			var users = new UsersServiceModel
			{
				Users = await usersRepo.All()
					.OrderBy(u => u.FirstName)
					.ThenBy(u => u.LastName)
					.Skip(PaginationConstants.UsersPerPage * (page - 1))
					.Take(PaginationConstants.UsersPerPage)
					.ProjectTo<UserServiceModel>(mapper.ConfigurationProvider)
					.ToListAsync(),
				TotalUsersCount = await usersRepo.All().CountAsync()
			};

			return users;
		}

		public async Task<UserAccountsAndCategoriesServiceModel> GetUserAccountsAndCategories(string userId)
		{
			if (!memoryCache.TryGetValue(CategoryConstants.CategoryCacheKeyValue + userId,
				out CategoryServiceModel[] categories))
			{
				categories = await categoriesRepo.All()
					.Where(c => c.OwnerId == userId && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => mapper.Map<CategoryServiceModel>(c))
					.ToArrayAsync();

				memoryCache.Set(CategoryConstants.CategoryCacheKeyValue + userId, categories, TimeSpan.FromDays(3));
			}

			if (!memoryCache.TryGetValue(AccountConstants.AccountCacheKeyValue + userId,
				out AccountServiceModel[] accounts))
			{
				accounts = await accountsRepo.All()
					.Where(a => a.OwnerId == userId && !a.IsDeleted)
					.OrderBy(a => a.Name)
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
				accTypes = await accountTypesRepo.All()
					.Where(at => at.OwnerId == userId && !at.IsDeleted)
					.OrderBy(at => at.Name)
					.Select(at => mapper.Map<AccountTypeServiceModel>(at))
					.ToArrayAsync();

				memoryCache.Set(AccountTypeConstants.AccTypeCacheKeyValue + userId, accTypes, TimeSpan.FromDays(3));
			}

			if (!memoryCache.TryGetValue(CurrencyConstants.CurrencyCacheKeyValue + userId,
				out CurrencyServiceModel[] currencies))
			{
				currencies = await currenciesRepo.All()
					.Where(c => c.OwnerId == userId && !c.IsDeleted)
					.OrderBy(c => c.Name)
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
			return await accountsRepo.All()
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => mapper.Map<AccountCardServiceModel>(a))
				.ToArrayAsync();
		}

		public async Task<int> GetUsersAccountsCount()
		{
			int accountsCount = await accountsRepo.All().CountAsync(a => !a.IsDeleted);

			return accountsCount;
		}

		public async Task<TransactionsServiceModel> GetUserTransactions(string userId, DateTime startDate, DateTime endDate, int page = 1)
		{
			TransactionsServiceModel userTransactions = await usersRepo.All()
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
			var dto = await usersRepo.All()
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
			var result = await usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsServiceModel>(mapper.ConfigurationProvider)
				.FirstAsync();

			return result;
		}

		public async Task<int> UsersCount() => await usersRepo.All().CountAsync();
	}
}