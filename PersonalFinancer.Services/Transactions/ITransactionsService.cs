using PersonalFinancer.Services.Transactions.Models;

namespace PersonalFinancer.Services.Transactions
{
	public interface ITransactionsService
	{
		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> CreateTransaction(TransactionFormModel transactionFormModel, bool isInitialBalance = false);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<decimal> DeleteTransaction(string transactionId);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditTransaction(string id, TransactionFormModel formModel);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionFormModel> GetTransactionFormModel(string transactionId);

		Task EditOrCreateInitialBalanceTransaction(string accountId, decimal amountOfChange);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionExtendedViewModel> GetTransactionViewModel(string transactionId);

		/// <summary>
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task GetUserTransactionsExtendedViewModel(string userId, UserTransactionsExtendedViewModel model);

		Task<IEnumerable<TransactionShortViewModel>> GetUserLastFiveTransactions(string userId, DateTime? startDate, DateTime? endDate);
	}
}
