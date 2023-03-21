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

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> CreateTransaction(
			string userId, TransactionFormModel transactionFormModel)
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

			await ChangeBalance(
				newTransaction.AccountId,
				newTransaction.Amount,
				newTransaction.TransactionType);

			await data.SaveChangesAsync();

			return newTransaction.Id;
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task ChangeBalance(
			string accountId, decimal amount, TransactionType transactionType)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
				throw new InvalidOperationException("Account does not exist.");

			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
		}

		private void ChangeBalance(Account account, decimal amount, TransactionType transactionType)
		{
			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
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
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditTransaction(string id, TransactionFormModel formModel)
		{
			Transaction transaction = await data.Transactions
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == id);

			bool isAccountOrAmountOrTransactionTypeChanged =
				formModel.AccountId != transaction.AccountId ||
				formModel.TransactionType != transaction.TransactionType ||
				formModel.Amount != transaction.Amount;

			if (isAccountOrAmountOrTransactionTypeChanged)
			{
				TransactionType newTransactionType = TransactionType.Income;

				if (transaction.TransactionType == TransactionType.Income)
					newTransactionType = TransactionType.Expense;

				ChangeBalance(transaction.Account, transaction.Amount, newTransactionType);
			}

			transaction.Refference = formModel.Refference.Trim();
			transaction.AccountId = formModel.AccountId;
			transaction.CategoryId = formModel.CategoryId;
			transaction.Amount = formModel.Amount;
			transaction.CreatedOn = formModel.CreatedOn;
			transaction.TransactionType = formModel.TransactionType;

			if (isAccountOrAmountOrTransactionTypeChanged)
			{
				await ChangeBalance(formModel.AccountId,
									formModel.Amount,
									formModel.TransactionType);
			}

			await data.SaveChangesAsync();
		}

		public async Task EditOrCreateInitialBalanceTransaction(string ownerId, string accountId, decimal amountOfChange)
		{
			Transaction? transaction = await data.Transactions.FirstOrDefaultAsync(t =>
				t.AccountId == accountId && t.IsInitialBalance);

			if (transaction == null)
			{
				await CreateTransaction(ownerId, new TransactionFormModel
				{
					AccountId = accountId,
					Amount = amountOfChange,
					CategoryId = InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow,
					Refference = CategoryInitialBalanceName,
					TransactionType = amountOfChange < 0 ?
						TransactionType.Expense
						: TransactionType.Income,
					IsInitialBalance = true
				});
			}
			else
			{
				transaction.Amount += amountOfChange;
			}

			await data.SaveChangesAsync();
		}

		/// <summary>
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task GetUserTransactionsExtendedViewModel
			(string userId, UserTransactionsExtendedViewModel model)
		{
			//TODO: Make Validation on DateFilterModel!
			if (model.StartDate > model.EndDate)
			{
				throw new ArgumentException("Start Date must be before End Date.");
			}

			model.TotalElements = data.Transactions.Count(t =>
				t.Account.OwnerId == userId
				&& t.CreatedOn >= model.StartDate
				&& t.CreatedOn <= model.EndDate);

			model.Transactions = await data.Transactions
				.Where(t =>
					t.Account.OwnerId == userId
					&& t.CreatedOn >= model.StartDate
					&& t.CreatedOn <= model.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.Skip(model.Page != 1 ?
					model.ElementsPerPage * (model.Page - 1)
					: 0)
				.Take(model.ElementsPerPage)
				.ProjectTo<TransactionExtendedViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();
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
					Accounts = t.Account.IsDeleted ?
						new List<AccountDropdownViewModel>()
						{
							new AccountDropdownViewModel { Id = t.AccountId, Name = t.Account.Name }
						}
						: t.Owner.Accounts.Select(a => new AccountDropdownViewModel
						{
							Id = a.Id,
							Name = a.Name
						}).ToList(),
					Categories = t.IsInitialBalance ?
						new List<CategoryViewModel>()
						{
							new CategoryViewModel { Id = t.CategoryId, Name = t.Category.Name }
						}
						: t.Owner.Categories.Select(c => new CategoryViewModel
						{
							Id = c.Id,
							Name = c.Name
						}).ToList()
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

		public async Task<IEnumerable<TransactionShortViewModel>> GetUserLastFiveTransactions
			(string userId, DateTime? startDate, DateTime? endDate)
		{
			return await data.Transactions
				.Where(t =>
					t.Account.OwnerId == userId &&
					t.CreatedOn >= startDate &&
					t.CreatedOn <= endDate)
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.ProjectTo<TransactionShortViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();
		}

		public async Task<TransactionFormModel> GetEmptyTransactionFormModel(string userId)
		{
			return await data.Users.Where(u => u.Id == userId)
				.Select(u => new TransactionFormModel
				{
					OwnerId = u.Id,
					CreatedOn = DateTime.UtcNow,
					Categories = u.Categories.Select(c => new CategoryViewModel
					{
						Id = c.Id,
						Name = c.Name
					}).ToList(),
					Accounts = u.Accounts.Select(a => new AccountDropdownViewModel
					{
						Id = a.Id,
						Name = a.Name
					}).ToList()
				})
				.FirstAsync();
		}
	}
}
