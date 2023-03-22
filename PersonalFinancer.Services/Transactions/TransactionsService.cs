using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Categories.Models;
using PersonalFinancer.Services.Transactions.Models;
using static PersonalFinancer.Data.Constants.CategoryConstants;

namespace PersonalFinancer.Services.Transactions
{
	public class TransactionsService : ITransactionsService
	{
		private PersonalFinancerDbContext data;
		private IMapper mapper;

		public TransactionsService(
			PersonalFinancerDbContext data,
			IMapper mapper)
		{
			this.data = data;
			this.mapper = mapper;
		}

		private void ChangeAccountBalance(Account account, decimal amount, TransactionType transactionType)
		{
			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task ChangeAccountBalanceAsync(string accountId, decimal amount, TransactionType transactionType)
		{
			Account account = await data.Accounts.FirstAsync(a => a.Id == accountId);

			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> CreateTransaction(string userId, TransactionFormModel transactionFormModel)
		{
			Transaction newTransaction = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				Amount = transactionFormModel.Amount,
				AccountId = transactionFormModel.AccountId,
				OwnerId = userId,
				CategoryId = transactionFormModel.CategoryId,
				TransactionType = transactionFormModel.TransactionType,
				CreatedOn = transactionFormModel.CreatedOn,
				Refference = transactionFormModel.Refference.Trim(),
				IsInitialBalance = transactionFormModel.IsInitialBalance
			};

			await data.Transactions.AddAsync(newTransaction);

			await ChangeAccountBalanceAsync(
				newTransaction.AccountId, 
				newTransaction.Amount, 
				newTransaction.TransactionType);

			await data.SaveChangesAsync();

			return newTransaction.Id;
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

			ChangeAccountBalance(transaction.Account, transaction.Amount, transaction.TransactionType);

			await data.SaveChangesAsync();

			return transaction.Account.Balance;
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

				ChangeAccountBalance(transactionInDb.Account, transactionInDb.Amount, opositeTransactionType);

				if (editedTransaction.AccountId != transactionInDb.AccountId)
				{
					Account newAccount = await data.Accounts.FirstAsync(a => a.Id == editedTransaction.AccountId);
					transactionInDb.Account = newAccount;
				}

				ChangeAccountBalance(transactionInDb.Account, editedTransaction.Amount, editedTransaction.TransactionType);
			}

			transactionInDb.Refference = editedTransaction.Refference.Trim();
			transactionInDb.AccountId = editedTransaction.AccountId;
			transactionInDb.CategoryId = editedTransaction.CategoryId;
			transactionInDb.Amount = editedTransaction.Amount;
			transactionInDb.CreatedOn = editedTransaction.CreatedOn;
			transactionInDb.TransactionType = editedTransaction.TransactionType;

			await data.SaveChangesAsync();
		}

		public async Task GetAllUserTransactions(string userId, UserTransactionsExtendedViewModel model)
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
				.ProjectTo<TransactionExtendedViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();
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
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TransactionExtendedViewModel> GetTransactionViewModel(string transactionId)
		{
			return await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionExtendedViewModel>(mapper.ConfigurationProvider)
				.FirstAsync();
		}
	}
}
