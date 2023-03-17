using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Shared.Models;
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

		public int GetUsersAccountsCount()
		{
			int accountsCount = data.Accounts.Count(a => !a.IsDeleted);

			return accountsCount;
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> GetOwnerId(string accountId)
		{
			Account account = await data.Accounts.FirstAsync(a => a.Id == accountId);

			return account.OwnerId;
		}

		public async Task<IEnumerable<AccountDropdownViewModel>> GetUserAccountsDropdownViewModel(string userId)
		{
			string cacheKey = AccountConstants.CacheKeyValue + userId;

			var accounts = await memoryCache.GetOrCreateAsync<IEnumerable<AccountDropdownViewModel>>(cacheKey, async cacheEntry =>
			{
				cacheEntry.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.Accounts
					.Where(a => a.OwnerId == userId && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Select(a => mapper.Map<AccountDropdownViewModel>(a))
					.ToArrayAsync();
			});

			return accounts;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountDropdownViewModel> GetAccountDropdownViewModel(string accountId)
		{
			return await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.FirstAsync();
		}

		public async Task<AllUsersAccountCardsViewModel> GetAllUsersAccountCardsViewModel(int page)
		{
			var model = new AllUsersAccountCardsViewModel();
			model.Pagination.Page = page;
			model.Pagination.TotalElements = data.Accounts.Count(a => !a.IsDeleted);
			model.Accounts = await data.Accounts
				.Where(a => !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Skip(model.Pagination.ElementsPerPage * (page - 1))
				.Take(model.Pagination.ElementsPerPage)
				.ProjectTo<AccountCardExtendedViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();

			return model;
		}

		public async Task<IEnumerable<AccountCardViewModel>> GetUserAccountCardsViewModel(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.ProjectTo<AccountCardViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<DetailsAccountViewModel> GetAccountDetailsViewModel(string accountId, DateTime startDate, DateTime endDate, int page = 1)
		{
			return await data.Accounts
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.Select(a => new DetailsAccountViewModel
				{
					Name = a.Name,
					Balance = a.Balance,
					CurrencyName = a.Currency.Name,
					Dates = new DateFilterModel
					{
						StartDate = startDate,
						EndDate = endDate
					},
					Pagination = new PaginationModel
					{
						Page = page,
						TotalElements = a.Transactions.Count(t => 
							t.CreatedOn >= startDate && t.CreatedOn <= endDate),
					},
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.OrderByDescending(t => t.CreatedOn)
						.Skip(page != 1 ? 10 * (page - 1) : 0)
						.Take(10)
						.Select(t => new AccountDetailsTransactionViewModel
						{
							Id = t.Id,
							Amount = t.Amount,
							CurrencyName = a.Currency.Name,
							CreatedOn = t.CreatedOn,
							CategoryName = t.Category.Name + (t.Category.IsDeleted ? 
								" (Deleted)" 
								: string.Empty),
							TransactionType = t.TransactionType.ToString(),
							Refference = t.Refference
						})
						.AsEnumerable()
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws ArgumentException when User already have Account with given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		public async Task<string> CreateAccount(string userId, AccountFormModel accountModel)
		{
			if (await IsNameExists(accountModel.Name, userId))
				throw new ArgumentException(
					$"The User already have Account with {accountModel.Name} name.");

			Account newAccount = new Account()
			{
				Id = Guid.NewGuid().ToString(),
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
					CategoryId = await categoryService
						.GetCategoryIdByName(CategoryConstants.CategoryInitialBalanceName),
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
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccount(
			string accountId, string userId, bool shouldDeleteTransactions)
		{
			Account? account = await data.Accounts
				.FirstAsync(a => a.Id == accountId && !a.IsDeleted);

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
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<DeleteAccountViewModel> GetDeleteAccountViewModel(string accountId)
		{
			return await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<DeleteAccountViewModel>(a))
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditAccount(string accountId, AccountFormModel model, string userId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
				throw new InvalidOperationException("Account does not exist.");

			if (account.Name != model.Name && await IsNameExists(model.Name, userId))
				throw new ArgumentException(
					$"The User already have Account with {model.Name} name.");

			account.Name = model.Name.Trim();
			account.CurrencyId = model.CurrencyId;
			account.AccountTypeId = model.AccountTypeId;

			if (account.Balance != model.Balance)
			{
				decimal amountOfChange = model.Balance - account.Balance;
				account.Balance = model.Balance;
				await transactionsService.EditOrCreateInitialBalanceTransaction(account.Id, amountOfChange);
			}
			else
			{
				await data.SaveChangesAsync();
			}
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountFormModel> GetEditAccountModel(string accountId)
		{
			return await data.Accounts
				.Where(a => a.Id == accountId)
				.ProjectTo<AccountFormModel>(mapper.ConfigurationProvider)
				.FirstAsync();
		}

		/// <summary>
		/// Throws ArgumentNullException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<bool> IsAccountOwner(string userId, string accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
				throw new InvalidOperationException("Account does not exist.");

			return account.OwnerId == userId;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<bool> IsAccountDeleted(string accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
				throw new InvalidOperationException("Account does not exist.");

			return account.IsDeleted;
		}

		public async Task<Dictionary<string, CashFlowViewModel>> GetUserAccountsCashFlow
			(string userId, DateTime? startDate, DateTime? endDate)
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
						result[a.Currency.Name].Incomes += (decimal)income;
					}

					decimal? expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						result[a.Currency.Name].Expenses += (decimal)expense;
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
						result[a.Currency.Name].Incomes += (decimal)income;
					}

					decimal? expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						result[a.Currency.Name].Expenses += (decimal)expense;
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