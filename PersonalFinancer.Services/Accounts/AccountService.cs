using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Services.Accounts
{
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

		public int AccountsCount()
		{
			int accountsCount = data.Accounts.Count(a => !a.IsDeleted);

			return accountsCount;
		}

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

		public async Task<AccountDropdownViewModel?> AccountDropdownViewModel(Guid accountId)
		{
			AccountDropdownViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.FirstOrDefaultAsync();

			return account;
		}

		public async Task<IEnumerable<AccountCardViewModel>> AllAccountsCardViewModel(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.ProjectTo<AccountCardViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();
		}

		public async Task<AccountDetailsViewModel?> AccountDetailsViewModel(Guid accountId)
		{
			AccountDetailsViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.ProjectTo<AccountDetailsViewModel>(mapper.ConfigurationProvider)
				.FirstOrDefaultAsync();

			return account;
		}

		public async Task<Guid> CreateAccount(string userId, AccountFormModel accountModel)
		{
			if (await IsNameExists(accountModel.Name, userId))
			{
				throw new InvalidOperationException($"The User already have Account with {accountModel.Name} name.");
			}

			Account newAccount = new Account()
			{
				Name = accountModel.Name.Trim(),
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

		public async Task<DeleteAccountViewModel?> DeleteAccountViewModel(Guid accountId)
		{
			DeleteAccountViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<DeleteAccountViewModel>(a))
				.FirstOrDefaultAsync();

			return account;
		}

		public async Task EditAccount(EditAccountFormModel accountModel, string userId)
		{
			Account? account = await data.Accounts.FindAsync(accountModel.Id);

			if (account == null)
			{
				throw new NullReferenceException("Account does not exist.");
			}

			if (account.Name != accountModel.Name && await IsNameExists(accountModel.Name, userId))
			{
				throw new InvalidOperationException($"The User already have Account with {accountModel.Name} name.");
			}

			account.Name = accountModel.Name.Trim();
			account.CurrencyId = accountModel.CurrencyId;
			account.AccountTypeId = accountModel.AccountTypeId;

			if (account.Balance != accountModel.Balance)
			{
				decimal amountOfChange = accountModel.Balance - account.Balance;
				account.Balance = accountModel.Balance;
				await transactionsService.EditInitialBalanceTransaction(account.Id, amountOfChange);
			}
			else
			{
				await data.SaveChangesAsync();
			}
		}

		public async Task<EditAccountFormModel> GetEditAccountFormModel(Guid accountId)
		{
			return await data.Accounts
				.Where(a => a.Id == accountId)
				.ProjectTo<EditAccountFormModel>(mapper.ConfigurationProvider)
				.FirstAsync();
		}

		public async Task<bool> IsAccountOwner(string userId, Guid accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			return account.OwnerId == userId;
		}

		public async Task<bool> IsAccountDeleted(Guid accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			return account.IsDeleted;
		}

		public async Task<Dictionary<string, CashFlowViewModel>> GetUserAccountsCashFlow(string userId, DateTime? startDate, DateTime? endDate)
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

		private async Task<bool> IsNameExists(string name, string userId)
		{
			var names = await data.Accounts
				.Where(a => a.OwnerId == userId)
				.Select(a => a.Name.ToLower())
				.ToArrayAsync();

			return names.Contains(name.ToLower().Trim());
		}
	}
}