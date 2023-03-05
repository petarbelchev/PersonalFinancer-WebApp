namespace PersonalFinancer.Services.Transactions
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;

	using Models;
	using Data;
	using Data.Enums;
	using Data.Models;

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
		/// Returns a collection of User's transactions for given period ordered by descending. 
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		/// <exception cref="ArgumentException"></exception>
		public async Task<AllTransactionsServiceModel> AllTransactionsViewModel(
			string userId, AllTransactionsServiceModel model)
		{
			if (model.StartDate > model.EndDate)
			{
				throw new ArgumentException("Start Date must be before End Date.");
			}

			TransactionExtendedViewModel[] transactions = await data.Transactions
				.Where(t =>
					t.Account.OwnerId == userId &&
					t.CreatedOn >= model.StartDate &&
					t.CreatedOn <= model.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.ProjectTo<TransactionExtendedViewModel>(mapper.ConfigurationProvider)
				.ToArrayAsync();

			model.Transactions = transactions;

			return model;
		}

		/// <summary>
		/// Creates a new Transaction and change account's balance if the transaction is not an initial balance transaction. 
		/// Returns new transaction's id.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<Guid> CreateTransaction(TransactionFormModel transactionFormModel, bool isInitialBalance = false)
		{
			Transaction newTransaction = new Transaction()
			{
				Amount = transactionFormModel.Amount,
				AccountId = transactionFormModel.AccountId,
				CategoryId = transactionFormModel.CategoryId,
				TransactionType = transactionFormModel.TransactionType,
				CreatedOn = transactionFormModel.CreatedOn,
				Refference = transactionFormModel.Refference
			};

			// TODO: Add Check if account have enough money for transaction?

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
		/// Changes balance on given Account or throws exception if Account does not exist.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
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

		/// <summary>
		/// Changes balance on given Account.
		/// </summary>
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
		/// Delete a Transaction and change account's balance. 
		/// </summary>
		/// <returns>New Account's balance</returns>
		/// <exception cref="ArgumentNullException">Throws an Exception when Transaction does not exist.</exception>
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

		/// <summary>
		/// Returns a Transaction with Id, AccountId, Amount, CategoryId, Refference, TransactionType, OwnerId, CreatedOn, or null.
		/// </summary>
		public async Task<EditTransactionFormModel?> EditTransactionFormModelById(Guid transactionId)
		{
			EditTransactionFormModel? transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<EditTransactionFormModel>(mapper.ConfigurationProvider)
				.FirstOrDefaultAsync();

			return transaction;
		}
		
		/// <summary>
		/// Edits a Transaction and change account's balance if it's nessesery.
		/// </summary>
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

				ChangeBalance(transaction.Account,
									transaction.Amount,
									newTransactionType);
			}

			transaction.Refference = editedTransaction.Refference;
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

		/// <summary>
		/// Returns Transaction Extended View Model with given Id, or null.
		/// </summary>
		public async Task<TransactionExtendedViewModel?> TransactionViewModel(Guid transactionId)
		{
			TransactionExtendedViewModel? transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionExtendedViewModel>(mapper.ConfigurationProvider)
				.FirstOrDefaultAsync();

			return transaction;
		}

		/// <summary>
		/// Returns Transaction Short View Model with last five user's transactions for given period.
		/// </summary>
		public async Task<IEnumerable<TransactionShortViewModel>> LastFiveTransactions(
			string userId, DateTime? startDate, DateTime? endDate)
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
