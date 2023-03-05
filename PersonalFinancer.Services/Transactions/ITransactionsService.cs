namespace PersonalFinancer.Services.Transactions
{
	using Models;
	using Data.Enums;
	using Data.Models;

	public interface ITransactionsService
	{
		/// <summary>
		/// Creates a new Transaction and change account's balance if the transaction is not an initial balance transaction. 
		/// Returns new transaction's id.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task<Guid> CreateTransaction(TransactionFormModel transactionFormModel, bool isInitialBalance = false);

		/// <summary>
		/// Delete a Transaction and change account's balance. 
		/// </summary>
		/// <returns>New Account's balance</returns>
		/// <exception cref="ArgumentNullException">Throws an Exception when Transaction does not exist.</exception>
		Task<decimal> DeleteTransactionById(Guid transactionId);

		/// <summary>
		/// Edits a Transaction and change account's balance if it's nessesery.
		/// </summary>
		Task EditTransaction(EditTransactionFormModel transactionFormModel);

		/// <summary>
		/// Returns a Transaction with Id, AccountId, Amount, CategoryId, Refference, TransactionType, OwnerId, CreatedOn, or null.
		/// </summary>
		Task<EditTransactionFormModel?> EditTransactionFormModelById(Guid transactionId);

		/// <summary>
		/// Returns Transaction Extended View Model with given Id, or null.
		/// </summary>
		Task<TransactionExtendedViewModel?> TransactionViewModel(Guid transactionId);

		/// <summary>
		/// Returns a collection of User's transactions for given period ordered by descending.
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		/// <exception cref="ArgumentException"></exception>
		Task<AllTransactionsServiceModel> AllTransactionsViewModel(string userId, AllTransactionsServiceModel model);
		
		/// <summary>
		/// Returns Transaction Short View Model with last five user's transactions for given period.
		/// </summary>
		Task<IEnumerable<TransactionShortViewModel>> LastFiveTransactions(string userId, DateTime? startDate, DateTime? endDate);
	}
}
