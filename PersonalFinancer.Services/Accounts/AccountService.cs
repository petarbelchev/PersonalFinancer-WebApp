namespace PersonalFinancer.Services.Accounts
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;

	using Models;
	using Category;
	using Transactions;
	using Transactions.Models;
	using Data;
	using Data.Enums;
	using Data.Models;
	using static Data.Constants;

	public class AccountService : IAccountService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly ITransactionsService transactionsService;
		private readonly ICategoryService categoryService;
		private readonly IMemoryCache memoryCache;

		public AccountService(
			PersonalFinancerDbContext context,
			IMapper mapper,
			ITransactionsService transactionsService,
			ICategoryService categoryService,
			IMemoryCache memoryCache)
		{
			this.data = context;
			this.mapper = mapper;
			this.transactionsService = transactionsService;
			this.categoryService = categoryService;
			this.memoryCache = memoryCache;
		}

		/// <summary>
		/// Returns count of all created accounts.
		/// </summary>
		public int AccountsCount()
		{
			int accountsCount = data.Accounts.Count(a => !a.IsDeleted);

			return accountsCount;
		}

		/// <summary>
		/// Returns collection of User's accounts with Id and Name.
		/// </summary>
		public async Task<IEnumerable<AccountDropdownViewModel>> AllAccountsDropdownViewModel(string userId)
		{
			string cacheKey = AccountConstants.CacheKeyValue + userId;

			var accounts = await memoryCache.GetOrCreateAsync<IEnumerable<AccountDropdownViewModel>>(cacheKey, async cacheEntry =>
			{
				cacheEntry.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.Accounts
					.Where(a => a.OwnerId == userId && !a.IsDeleted)
					.Select(a => mapper.Map<AccountDropdownViewModel>(a))
					.ToArrayAsync();
			});

			return accounts;
		}

		/// <summary>
		/// Returns Account with Id and Name or null.
		/// </summary>
		public async Task<AccountDropdownViewModel?> AccountDropdownViewModel(Guid accountId)
		{
			AccountDropdownViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.FirstOrDefaultAsync();

			return account;
		}

		/// <summary>
		/// Returns a collection of user's accounts with Id, Name, Balance and Currency Name.
		/// </summary>
		public async Task<IEnumerable<AccountCardViewModel>> AllAccountsCardViewModel(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.ProjectTo<AccountCardViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();
		}

		/// <summary>
		/// Returns Account with Id, Name, Balance and all Transactions or null.
		/// </summary>
		public async Task<AccountDetailsViewModel?> AccountDetailsViewModel(Guid accountId)
		{
			AccountDetailsViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.ProjectTo<AccountDetailsViewModel>(mapper.ConfigurationProvider)
				.FirstOrDefaultAsync();

			return account;
		}

		/// <summary>
		/// Returns collection of Account Types for the current user with Id and Name.
		/// </summary>
		public async Task<IEnumerable<AccountTypeViewModel>> AccountTypesViewModel(string userId)
		{
			return await data.AccountTypes
				.Where(a => (a.UserId == null || a.UserId == userId) && !a.IsDeleted)
				.Select(a => mapper.Map<AccountTypeViewModel>(a))
				.ToArrayAsync();
		}

		/// <summary>
		/// Creates a new Account and if the new account has initial balance creates new Transaction with given amount.
		/// Returns new Account's id.
		/// </summary>
		public async Task<Guid> CreateAccount(string userId, AccountFormModel accountModel)
		{
			Account newAccount = new Account()
			{
				Name = accountModel.Name,
				Balance = accountModel.Balance,
				AccountTypeId = accountModel.AccountTypeId,
				CurrencyId = accountModel.CurrencyId,
				OwnerId = userId
			};

			await data.Accounts.AddAsync(newAccount);

			if (newAccount.Balance != 0)
			{
				await transactionsService.CreateTransaction(new TransactionFormModel
				{
					Amount = newAccount.Balance,
					AccountId = newAccount.Id,
					CategoryId = await categoryService.CategoryIdByName(CategoryConstants.CategoryInitialBalanceName),
					Refference = "Initial Balance",
					CreatedOn = DateTime.UtcNow,
					TransactionType = TransactionType.Income
				}, true);
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.CacheKeyValue + userId);

			return newAccount.Id;
		}

		/// <summary>
		/// Delete an Account and give the option to delete all of the account's transactions.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task DeleteAccountById(Guid accountId, string userId, bool shouldDeleteTransactions)
		{
			Account? account = await data.Accounts
				.FirstOrDefaultAsync(a => a.Id == accountId && !a.IsDeleted);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			if (shouldDeleteTransactions)
			{
				data.Accounts.Remove(account);
			}
			else
			{
				account.IsDeleted = true;
			}

			await data.SaveChangesAsync();
			
			memoryCache.Remove(AccountConstants.CacheKeyValue + userId);
		}

		/// <summary>
		/// Returns Delete Account View Model.
		/// </summary>
		public async Task<DeleteAccountViewModel?> DeleteAccountViewModel(Guid accountId)
		{
			DeleteAccountViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<DeleteAccountViewModel>(a))
				.FirstOrDefaultAsync();

			return account;
		}

		/// <summary>
		/// Checks is the given User is owner of the given account, if does not exist, throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<bool> IsAccountOwner(string userId, Guid accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			return account.OwnerId == userId;
		}

		/// <summary>
		/// Checks is the given Account deleted, if does not exist, throws an exception.
		/// </summary>
		/// <param name="accountId"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<bool> IsAccountDeleted(Guid accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			return account.IsDeleted;
		}

		/// <summary>
		/// Returns User's accounts Cash Flow for a given period.
		/// </summary>
		public async Task<Dictionary<string, CashFlowViewModel>> GetUserAccountsCashFlow(
			string userId, DateTime? startDate, DateTime? endDate)
		{
			var result = new Dictionary<string, CashFlowViewModel>();

			await data.Accounts
				.Where(a => a.OwnerId == userId
					&& a.Transactions.Any(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate))
				.Include(a => a.Currency)
				.Include(a => a.Transactions
					.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate))
				.ForEachAsync(a =>
				{
					if (!result.ContainsKey(a.Currency.Name))
					{
						result[a.Currency.Name] = new CashFlowViewModel();
					}

					decimal? income = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount);

					if (income != null)
					{
						result[a.Currency.Name].Income += (decimal)income;
					}

					decimal? expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						result[a.Currency.Name].Expence += (decimal)expense;
					}
				});

			return result;
		}

		/// <summary>
		/// Returns Cash Flow of all user's accounts.
		/// </summary>
		/// <returns></returns>
		public async Task<Dictionary<string, CashFlowViewModel>> GetAllAccountsCashFlow()
		{
			var result = new Dictionary<string, CashFlowViewModel>();

			await data.Accounts
				.Where(a => a.Transactions.Any())
				.Include(a => a.Currency)
				.Include(a => a.Transactions)
				.ForEachAsync(a =>
				{
					if (!result.ContainsKey(a.Currency.Name))
					{
						result[a.Currency.Name] = new CashFlowViewModel();
					}

					decimal? income = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount);

					if (income != null)
					{
						result[a.Currency.Name].Income += (decimal)income;
					}

					decimal? expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						result[a.Currency.Name].Expence += (decimal)expense;
					}
				});

			return result;
		}
	}
}