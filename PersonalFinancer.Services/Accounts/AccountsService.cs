﻿using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Services.Accounts
{
    public class AccountsService : IAccountsService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public AccountsService(
			PersonalFinancerDbContext context,
			IMapper mapper,
			IMemoryCache memoryCache)
		{
			this.data = context;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
		}

		private void ChangeBalance(Account account, decimal amount, TransactionType transactionType)
		{
			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
		}

		/// <summary>
		/// Throws ArgumentException when User already have Account with given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		public async Task<string> CreateAccount(AccountFormModel model)
		{
			if (await IsNameExists(model.Name, model.OwnerId))
				throw new ArgumentException(
					$"The User already have Account with {model.Name} name.");

			Account newAccount = new Account()
			{
				Id = Guid.NewGuid().ToString(),
				Name = model.Name.Trim(),
				Balance = model.Balance ?? // Not supposed to be null, its nullable because of model validation
					throw new InvalidOperationException("Account balance cannot be null."),
				AccountTypeId = model.AccountTypeId,
				CurrencyId = model.CurrencyId,
				OwnerId = model.OwnerId
			};

			if (newAccount.Balance != 0)
			{
				newAccount.Transactions.Add(new Transaction()
				{
					Id = Guid.NewGuid().ToString(),
					Amount = newAccount.Balance,
					OwnerId = newAccount.OwnerId,
					CategoryId = TransactionConstants.InitialBalanceCategoryId,
					TransactionType = TransactionType.Income,
					CreatedOn = DateTime.UtcNow,
					Refference = TransactionConstants.CategoryInitialBalanceName,
					IsInitialBalance = true
				});
			}

			await data.Accounts.AddAsync(newAccount);
			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.CacheKeyValue + model.OwnerId);

			return newAccount.Id;
		}
		
		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> CreateTransaction(string userId, TransactionFormModel transactionFormModel)
		{
			Account? account = await data.Accounts.FindAsync(transactionFormModel.AccountId);

			if (account == null)
				throw new InvalidOperationException("Account does not exist.");

			Transaction newTransaction = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				Amount = transactionFormModel.Amount,
				OwnerId = userId,
				CategoryId = transactionFormModel.CategoryId,
				TransactionType = transactionFormModel.TransactionType,
				CreatedOn = transactionFormModel.CreatedOn,
				Refference = transactionFormModel.Refference.Trim(),
				IsInitialBalance = transactionFormModel.IsInitialBalance
			};

			account.Transactions.Add(newTransaction);

			if (transactionFormModel.TransactionType == TransactionType.Income)
				account.Balance += newTransaction.Amount;
			else if (transactionFormModel.TransactionType == TransactionType.Expense)
				account.Balance -= newTransaction.Amount;

			await data.SaveChangesAsync();

			return newTransaction.Id;
		}
			
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccount(string accountId, bool shouldDeleteTransactions = false, string? userId = null)
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
		/// Throws InvalidOperationException when Transaction does not exist
		/// and ArgumentException when Owner Id is passed and User is not owner.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<decimal> DeleteTransaction(string transactionId, string? ownerId = null)
		{
			Transaction transaction = await data.Transactions
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == transactionId);

			if (ownerId != null && transaction.OwnerId != ownerId)
				throw new ArgumentException("User is now transaction's owner");

			data.Transactions.Remove(transaction);

			if (transaction.TransactionType == TransactionType.Income)
				transaction.TransactionType = TransactionType.Expense;
			else
				transaction.TransactionType = TransactionType.Income;

			ChangeBalance(transaction.Account, transaction.Amount, transaction.TransactionType);

			await data.SaveChangesAsync();

			return transaction.Account.Balance;
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
						Amount = amountOfChange,
						CategoryId = TransactionConstants.InitialBalanceCategoryId,
						CreatedOn = DateTime.UtcNow,
						Refference = TransactionConstants.CategoryInitialBalanceName,
						TransactionType = amountOfChange < 0 ? TransactionType.Expense : TransactionType.Income,
						IsInitialBalance = true
					};

					account.Transactions.Add(initialBalance);
				}
				else
				{
					transaction.Amount += amountOfChange;
				}
			}

			await data.SaveChangesAsync();
		}
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction or Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditTransaction(string id, TransactionFormModel editedTransaction)
		{
			Transaction transactionInDb = await data.Transactions
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == id);

			if (editedTransaction.AccountId != transactionInDb.AccountId
				|| editedTransaction.TransactionType != transactionInDb.TransactionType
				|| editedTransaction.Amount != transactionInDb.Amount)
			{
				TransactionType opositeTransactionType = TransactionType.Income;

				if (transactionInDb.TransactionType == TransactionType.Income)
					opositeTransactionType = TransactionType.Expense;

				ChangeBalance(transactionInDb.Account, transactionInDb.Amount, opositeTransactionType);

				if (editedTransaction.AccountId != transactionInDb.AccountId)
				{
					Account newAccount = await data.Accounts.FirstAsync(a => a.Id == editedTransaction.AccountId);
					transactionInDb.Account = newAccount;
				}

				ChangeBalance(transactionInDb.Account, editedTransaction.Amount, editedTransaction.TransactionType);
			}

			transactionInDb.Refference = editedTransaction.Refference.Trim();
			transactionInDb.AccountId = editedTransaction.AccountId;
			transactionInDb.CategoryId = editedTransaction.CategoryId;
			transactionInDb.Amount = editedTransaction.Amount;
			transactionInDb.CreatedOn = editedTransaction.CreatedOn;
			transactionInDb.TransactionType = editedTransaction.TransactionType;

			await data.SaveChangesAsync();
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
						.Select(t => new TransactionTableViewModel
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = a.Currency.Name,
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
		public async Task<AccountDropdownViewModel> GetAccountDropdownViewModel(string accountId)
		{
			return await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.FirstAsync();
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
					Currencies = a.Owner.Currencies
						.Where(c => !c.IsDeleted)
						.Select(c => new CurrencyViewModel
						{
							Id = c.Id,
							Name = c.Name
						}),
					AccountTypes = a.Owner.AccountTypes
						.Where(at => !at.IsDeleted)
						.Select(at => new AccountTypeViewModel
						{
							Id = at.Id,
							Name = at.Name
						})
				})
				.FirstAsync();
		}

		public async Task<Dictionary<string, CashFlowViewModel>> GetUsersAccountsCashFlow()
		{
			//TODO: Need refactoring. Use GroupBy? SelectMany? IQueryable variables?

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
		
		public async Task<UsersAccountCardsViewModel> GetUsersAccountCardsViewModel(int page)
		{
			var model = new UsersAccountCardsViewModel();
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
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountFormModel> GetEmptyAccountFormModel(string userId)
		{
			return await data.Users.Where(u => u.Id == userId)
				.Select(u => new AccountFormModel
				{
					OwnerId = u.Id,
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

		public async Task<TransactionFormModel> GetEmptyTransactionFormModel(string userId)
		{
			return await data.Users.Where(u => u.Id == userId)
				.Select(u => new TransactionFormModel
				{
					OwnerId = u.Id,
					CreatedOn = DateTime.UtcNow,
					UserAccounts = u.Accounts
						.Where(a => !a.IsDeleted)
						.Select(a => mapper.Map<AccountDropdownViewModel>(a)),
					UserCategories = u.Categories
						.Where(c => !c.IsDeleted)
						.Select(c => mapper.Map<CategoryViewModel>(c))
				})
				.FirstAsync();
		}	
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TransactionFormModel> GetFulfilledTransactionFormModel(string transactionId)
		{
			return await data.Transactions.Where(t => t.Id == transactionId)
				.Select(t => new TransactionFormModel
				{
					OwnerId = t.OwnerId,
					AccountId = t.AccountId,
					CategoryId = t.CategoryId,
					Amount = t.Amount,
					CreatedOn = t.CreatedOn,
					TransactionType = t.TransactionType,
					Refference = t.Refference,
					UserAccounts = t.Account.IsDeleted ?
						new List<AccountDropdownViewModel>()
						{
							new AccountDropdownViewModel { Id = t.AccountId, Name = t.Account.Name }
						}
						: t.Owner.Accounts
							.Where(a => !a.IsDeleted)
							.Select(a => mapper.Map<AccountDropdownViewModel>(a)),
					UserCategories = t.IsInitialBalance ?
						new List<CategoryViewModel>()
						{
							new CategoryViewModel { Id = t.CategoryId, Name = t.Category.Name }
						}
						: t.Owner.Categories.Select(c => mapper.Map<CategoryViewModel>(c))
				})
				.FirstAsync();
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
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TransactionDetailsViewModel> GetTransactionViewModel(string transactionId)
		{
			return await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionDetailsViewModel>(mapper.ConfigurationProvider)
				.FirstAsync();
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
		
		private async Task<bool> IsNameExists(string name, string userId)
		{
			var names = await data.Accounts
				.Where(a => a.OwnerId == userId)
				.Select(a => a.Name.ToLower())
				.ToArrayAsync();

			return names.Contains(name.ToLower().Trim());
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task PrepareAccountDetailsViewModelForReturn(string accountId, DetailsAccountViewModel model)
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
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task PrepareAccountFormModelForReturn(AccountFormModel model)
		{
			AccountFormModel emptyFormModel = await GetEmptyAccountFormModel(model.OwnerId);

			model.AccountTypes = emptyFormModel.AccountTypes;
			model.Currencies = emptyFormModel.Currencies;
		}

		public async Task SetUserTransactionsViewModel(string userId, UserTransactionsViewModel model)
		{
			model.Pagination.TotalElements = data.Transactions.Count(t =>
				t.Account.OwnerId == userId
				&& t.CreatedOn >= model.StartDate
				&& t.CreatedOn <= model.EndDate);

			model.Transactions = await data.Transactions
				.Where(t =>
					t.Account.OwnerId == userId
					&& t.CreatedOn >= model.StartDate
					&& t.CreatedOn <= model.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.Skip(model.Pagination.Page != 1 ?
					model.Pagination.ElementsPerPage * (model.Pagination.Page - 1)
					: 0)
				.Take(model.Pagination.ElementsPerPage)
				.ProjectTo<TransactionTableViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();
		}
	}
}