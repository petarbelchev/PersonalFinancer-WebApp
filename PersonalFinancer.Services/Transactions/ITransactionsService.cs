using PersonalFinancer.Services.Transactions.Models;

namespace PersonalFinancer.Services.Transactions
{
	public interface ITransactionsService
	{
		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> CreateTransaction(string userId, TransactionFormModel transactionFormModel);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist
		/// and ArgumentException when Owner Id is passed and User is not owner.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<decimal> DeleteTransaction(string transactionId, string? ownerId = null);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction or Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditTransaction(string id, TransactionFormModel editedTransaction);

		Task GetAllUserTransactions(string userId, UserTransactionsExtendedViewModel model);

		Task<TransactionFormModel> GetEmptyTransactionFormModel(string userId);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionFormModel> GetFulfilledTransactionFormModel(string transactionId);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionExtendedViewModel> GetTransactionViewModel(string transactionId);
	}
}
