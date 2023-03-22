using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.Currencies.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.Transactions;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Services.Accounts
{
	public class AccountService : IAccountService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly ITransactionsService transactionsService;
		private readonly IMemoryCache memoryCache;

		public AccountService(
			PersonalFinancerDbContext context,
			IMapper mapper,
			ITransactionsService transactionsService,
			IMemoryCache memoryCache)
		{
			this.data = context;
			this.mapper = mapper;
			this.transactionsService = transactionsService;
			this.memoryCache = memoryCache;
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

		//TODO: Move it to User Service?
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

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<DetailsAccountViewModel> GetAccountDetailsViewModel(
			string accountId, DateTime startDate, DateTime endDate, int page = 1, string? ownerId = null)
		{
			bool isUserAdmin = false;

			if (ownerId == null)
				isUserAdmin = true;

			return await data.Accounts
				.Where(a => a.Id == accountId
							&& !a.IsDeleted
							&& (isUserAdmin || a.OwnerId == ownerId))
				.Select(a => new DetailsAccountViewModel
				{
					Name = a.Name,
					Balance = a.Balance,
					CurrencyName = a.Currency.Name,
					StartDate = startDate,
					EndDate = endDate,
					Pagination = new PaginationModel
					{
						Page = page,
						TotalElements = a.Transactions.Count(t =>
							t.CreatedOn >= startDate && t.CreatedOn <= endDate)
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
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task PrepareAccountDetailsViewModelForReturn(
			string accountId, DetailsAccountViewModel model)
		{
			var dto = await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => new DetailsAccountViewModel
				{
					Name = a.Name,
					Balance = a.Balance,
					CurrencyName = a.Currency.Name
				})
				.FirstAsync();

			model.Name = dto.Name;
			model.Balance = dto.Balance;
			model.CurrencyName = dto.CurrencyName;
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
				Balance = accountModel.Balance ?? throw new InvalidOperationException("Account balance cannot be null."),
				AccountTypeId = accountModel.AccountTypeId,
				CurrencyId = accountModel.CurrencyId,
				OwnerId = userId
			};

			await data.Accounts.AddAsync(newAccount);

			if (newAccount.Balance != 0)
			{
				await data.Transactions.AddAsync(new Transaction()
				{
					Id = Guid.NewGuid().ToString(),
					Amount = newAccount.Balance,
					AccountId = newAccount.Id,
					OwnerId = newAccount.OwnerId,
					CategoryId = CategoryConstants.InitialBalanceCategoryId,
					TransactionType = TransactionType.Income,
					CreatedOn = DateTime.UtcNow,
					Refference = CategoryConstants.CategoryInitialBalanceName,
					IsInitialBalance = true
				});
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.CacheKeyValue + userId);

			return newAccount.Id;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccount(
			string accountId, bool shouldDeleteTransactions = false, string? userId = null)
		{
			Account account = await data.Accounts
				.FirstAsync(a => a.Id == accountId && !a.IsDeleted);

			if (account.OwnerId != userId)
				throw new ArgumentException("Can't delete someone else account.");

			if (shouldDeleteTransactions)
				data.Accounts.Remove(account);
			else
				account.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.CacheKeyValue + userId);
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<DeleteAccountViewModel> GetDeleteAccountViewModel(string accountId, string? ownerId = null)
		{
			bool isUserAdmin = false;

			if (ownerId == null)
				isUserAdmin = true;

			return await data.Accounts
				.Where(a => a.Id == accountId
							&& (isUserAdmin || a.OwnerId == ownerId))
				.Select(a => mapper.Map<DeleteAccountViewModel>(a))
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditAccount(string accountId, AccountFormModel model)
		{
			Account account = await data.Accounts.FirstAsync(a => a.Id == accountId);

			if (account.Name != model.Name && await IsNameExists(model.Name, model.OwnerId))
				throw new ArgumentException($"The User already have Account with {model.Name} name.");

			account.Name = model.Name.Trim();
			account.CurrencyId = model.CurrencyId;
			account.AccountTypeId = model.AccountTypeId;

			if (account.Balance != model.Balance)
			{
				decimal amountOfChange = (model.Balance ?? throw new InvalidOperationException("Account balance cannot be null.")) - account.Balance;
				account.Balance = model.Balance ?? throw new InvalidOperationException("Account balance cannot be null.");

				Transaction? transaction = await data.Transactions
					.FirstOrDefaultAsync(t => t.AccountId == account.Id && t.IsInitialBalance);

				if (transaction == null)
				{
					var initialBalance = new Transaction
					{
						Id = Guid.NewGuid().ToString(),
						OwnerId = account.OwnerId,
						AccountId = account.Id,
						Amount = amountOfChange,
						CategoryId = CategoryConstants.InitialBalanceCategoryId,
						CreatedOn = DateTime.UtcNow,
						Refference = CategoryConstants.CategoryInitialBalanceName,
						TransactionType = amountOfChange < 0 ? TransactionType.Expense : TransactionType.Income,
						IsInitialBalance = true
					};

					await data.Transactions.AddAsync(initialBalance);
				}
				else
				{
					transaction.Amount += amountOfChange;
				}
			}

			await data.SaveChangesAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist 
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountFormModel> GetAccountFormModel(string accountId, string? ownerId = null)
		{
			bool isUserAdmin = false;

			if (ownerId == null)
				isUserAdmin = true;

			return await data.Accounts
				.Where(a => a.Id == accountId
							&& (isUserAdmin || a.OwnerId == ownerId))
				.Select(a => new AccountFormModel
				{
					Name = a.Name,
					OwnerId = a.OwnerId,
					CurrencyId = a.CurrencyId,
					AccountTypeId = a.AccountTypeId,
					Balance = a.Balance,
					Currencies = a.Owner.Currencies.Select(c => new CurrencyViewModel
					{
						Id = c.Id,
						Name = c.Name
					}),
					AccountTypes = a.Owner.AccountTypes.Select(at => new AccountTypeViewModel
					{
						Id = at.Id,
						Name = at.Name
					})
				})
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

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountFormModel> GetEmptyAccountFormModel(string userId)
		{
			return await data.Users.Where(u => u.Id == userId)
				.Select(u => new AccountFormModel
				{
					AccountTypes = u.AccountTypes
						.Where(at => !at.IsDeleted)
						.Select(at => new AccountTypeViewModel
						{
							Id = at.Id,
							Name = at.Name
						}),
					Currencies = u.Currencies
						.Where(c => !c.IsDeleted)
						.Select(c => new CurrencyViewModel
						{
							Id = c.Id,
							Name = c.Name
						})
				})
				.FirstAsync();
		}
	}
}