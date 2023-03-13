using PersonalFinancer.Services.Transactions.Models;

namespace PersonalFinancer.Services.Transactions
{
	public interface ITransactionsService
	{
		Task<Guid> CreateTransaction(CreateTransactionFormModel transactionFormModel, bool isInitialBalance = false);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<decimal> DeleteTransaction(Guid transactionId);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditTransaction(EditTransactionFormModel transactionFormModel);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<EditTransactionFormModel> GetEditTransactionFormModel(Guid transactionId);

		Task EditOrCreateInitialBalanceTransaction(Guid accountId, decimal amountOfChange);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionExtendedViewModel> GetTransactionViewModel(Guid transactionId);

		/// <summary>
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task GetUserTransactionsExtendedViewModel(string userId, UserTransactionsExtendedViewModel model);

		Task<IEnumerable<TransactionShortViewModel>> GetUserLastFiveTransactions(string userId, DateTime? startDate, DateTime? endDate);
	}
}
