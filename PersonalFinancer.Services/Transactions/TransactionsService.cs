using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Transactions.Models;
using static PersonalFinancer.Data.Constants.SeedConstants;
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

		public async Task<Guid> CreateTransaction
			(CreateTransactionFormModel transactionFormModel, bool isInitialBalance = false)
		{
			Transaction newTransaction = new Transaction()
			{
				Amount = transactionFormModel.Amount,
				AccountId = transactionFormModel.AccountId,
				CategoryId = transactionFormModel.CategoryId,
				TransactionType = transactionFormModel.TransactionType,
				CreatedOn = transactionFormModel.CreatedOn,
				Refference = transactionFormModel.Refference.Trim()
			};

			await data.Transactions.AddAsync(newTransaction);

			if (!isInitialBalance)
			{
				await ChangeBalance(newTransaction.AccountId,
								newTransaction.Amount,
								newTransaction.TransactionType);

				await data.SaveChangesAsync();
			}

			return newTransaction.Id;
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task ChangeBalance
			(Guid accountId, decimal amount, TransactionType transactionType)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new InvalidOperationException("Account does not exist.");
			}

			if (transactionType == TransactionType.Income)
			{
				account.Balance += amount;
			}
			else if (transactionType == TransactionType.Expense)
			{
				account.Balance -= amount;
			}
		}

		private void ChangeBalance(Account account, decimal amount, TransactionType transactionType)
		{
			if (transactionType == TransactionType.Income)
			{
				account.Balance += amount;
			}
			else if (transactionType == TransactionType.Expense)
			{
				account.Balance -= amount;
			}
		}

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<decimal> DeleteTransaction(Guid transactionId)
		{
			Transaction transaction = await data.Transactions
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == transactionId);

			data.Transactions.Remove(transaction);

			if (transaction.TransactionType == TransactionType.Income)
			{
				transaction.TransactionType = TransactionType.Expense;
			}
			else
			{
				transaction.TransactionType = TransactionType.Income;
			}

			ChangeBalance(transaction.Account,
								transaction.Amount,
								transaction.TransactionType);

			await data.SaveChangesAsync();

			return transaction.Account.Balance;
		}
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditTransaction(EditTransactionFormModel editedTransaction)
		{
			Transaction transaction = await data.Transactions
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == editedTransaction.Id);

			bool isAccountOrAmountOrTransactionTypeChanged =
				editedTransaction.AccountId != transaction.AccountId ||
				editedTransaction.TransactionType != transaction.TransactionType ||
				editedTransaction.Amount != transaction.Amount;

			if (isAccountOrAmountOrTransactionTypeChanged)
			{
				TransactionType newTransactionType = TransactionType.Income;

				if (transaction.TransactionType == TransactionType.Income)
				{
					newTransactionType = TransactionType.Expense;
				}

				ChangeBalance(transaction.Account, transaction.Amount, newTransactionType);
			}

			transaction.Refference = editedTransaction.Refference.Trim();
			transaction.AccountId = editedTransaction.AccountId;
			transaction.CategoryId = editedTransaction.CategoryId;
			transaction.Amount = editedTransaction.Amount;
			transaction.CreatedOn = editedTransaction.CreatedOn;
			transaction.TransactionType = editedTransaction.TransactionType;

			if (isAccountOrAmountOrTransactionTypeChanged)
			{
				await ChangeBalance(editedTransaction.AccountId,
									editedTransaction.Amount,
									editedTransaction.TransactionType);
			}

			await data.SaveChangesAsync();
		}

		public async Task EditOrCreateInitialBalanceTransaction(Guid accountId, decimal amountOfChange)
		{
			Transaction? transaction = await data.Transactions
				.FirstOrDefaultAsync(t => t.AccountId == accountId && t.CategoryId == Guid.Parse(InitialBalanceCategoryId));

			if (transaction == null)
			{
				await CreateTransaction(new CreateTransactionFormModel
				{
					AccountId = accountId,
					Amount = amountOfChange,
					CategoryId = Guid.Parse(InitialBalanceCategoryId),
					CreatedOn = DateTime.UtcNow,
					Refference = CategoryInitialBalanceName,
					TransactionType = amountOfChange < 0 ? TransactionType.Expense : TransactionType.Income
				}, isInitialBalance: true);
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
			if (model.Dates.StartDate > model.Dates.EndDate)
			{
				throw new ArgumentException("Start Date must be before End Date.");
			}

			model.Pagination.TotalElements = data.Transactions
				.Count(t =>
					t.Account.OwnerId == userId &&
					t.CreatedOn >= model.Dates.StartDate &&
					t.CreatedOn <= model.Dates.EndDate);

			model.Transactions = await data.Transactions
				.Where(t =>
					t.Account.OwnerId == userId &&
					t.CreatedOn >= model.Dates.StartDate &&
					t.CreatedOn <= model.Dates.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.Skip(model.Pagination.Page != 1 ? model.Pagination.ElementsPerPage * (model.Pagination.Page - 1) : 0)
				.Take(model.Pagination.ElementsPerPage)
				.ProjectTo<TransactionExtendedViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();
		}
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<EditTransactionFormModel> GetEditTransactionFormModel(Guid transactionId)
		{
			return await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<EditTransactionFormModel>(mapper.ConfigurationProvider)
				.FirstAsync();
		}
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TransactionExtendedViewModel> GetTransactionViewModel(Guid transactionId)
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
	}
}
