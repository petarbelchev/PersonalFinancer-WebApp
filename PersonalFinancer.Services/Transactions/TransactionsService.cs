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

		public async Task<AllTransactionsServiceModel> AllTransactionsServiceModel(string userId, AllTransactionsServiceModel model)
		{
			if (model.StartDate > model.EndDate)
			{
				throw new ArgumentException("Start Date must be before End Date.");
			}

			model.TotalTransactions = data.Transactions
				.Count(t =>
					t.Account.OwnerId == userId &&
					t.CreatedOn >= model.StartDate &&
					t.CreatedOn <= model.EndDate);

			model.Transactions = await data.Transactions
				.Where(t =>
					t.Account.OwnerId == userId &&
					t.CreatedOn >= model.StartDate &&
					t.CreatedOn <= model.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.Skip(model.Page != 1 ? model.TransactionsPerPage * (model.Page - 1) : 0)
				.Take(model.TransactionsPerPage)
				.ProjectTo<TransactionExtendedViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();

			return model;
		}

		public async Task<Guid> CreateTransaction(TransactionFormModel transactionFormModel, bool isInitialBalance = false)
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

		private async Task ChangeBalance(Guid accountId, decimal amount, TransactionType transactionType)
		{
			Account account = await data.Accounts.FirstAsync(a => a.Id == accountId);

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

		public async Task<decimal> DeleteTransactionById(Guid transactionId)
		{
			Transaction? transaction = await data.Transactions
				.Include(t => t.Account)
				.FirstOrDefaultAsync(t => t.Id == transactionId);

			if (transaction == null)
			{
				throw new ArgumentNullException(nameof(transactionId), "Transaction does not exist.");
			}

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

		public async Task<EditTransactionFormModel?> EditTransactionFormModelById(Guid transactionId)
		{
			EditTransactionFormModel? transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<EditTransactionFormModel>(mapper.ConfigurationProvider)
				.FirstOrDefaultAsync();

			return transaction;
		}

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

		public async Task EditInitialBalanceTransaction(Guid accountId, decimal amountOfChange)
		{
			Transaction? transaction = await data.Transactions
				.FirstOrDefaultAsync(t => t.AccountId == accountId && t.CategoryId == Guid.Parse(InitialBalanceCategoryId));

			if (transaction == null)
			{
				await CreateTransaction(new TransactionFormModel
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

		public async Task<TransactionExtendedViewModel?> TransactionViewModel(Guid transactionId)
		{
			TransactionExtendedViewModel? transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionExtendedViewModel>(mapper.ConfigurationProvider)
				.FirstOrDefaultAsync();

			return transaction;
		}

		public async Task<IEnumerable<TransactionShortViewModel>> LastFiveTransactions(string userId, DateTime? startDate, DateTime? endDate)
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
