namespace PersonalFinancer.Services.Accounts
{
    using Services.Accounts.Models;
    using Services.Shared.Models;

    public interface IAccountsService
	{
		/// <summary>
		/// Throws ArgumentException when User already have Account with the given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		Task<string> CreateAccount(AccountFormShortServiceModel model);

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <returns>New transaction Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> CreateTransaction(TransactionFormShortServiceModel model);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccount(string accountId, string userId, bool isUserAdmin, bool shouldDeleteTransactions = false);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<decimal> DeleteTransaction(string transactionId, string userId, bool isUserAdmin);

		/// <summary>
		/// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditAccount(string accountId, AccountFormShortServiceModel model);

		/// <summary>
		/// Throws InvalidOperationException when Transaction or Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditTransaction(string id, TransactionFormShortServiceModel model);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDetailsServiceModel> GetAccountDetails(
			string id, DateTime startDate, DateTime endDate, string ownerId, bool isUserAdmin);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist 
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountFormServiceModel> GetAccountFormData(string accountId, string userId, bool isUserAdmin);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionsServiceModel> GetAccountTransactions(
			string id, DateTime startDate, DateTime endDate, int page);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> GetAccountName(string accountId, string userId, bool isUserAdmin);

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionFormServiceModel> GetTransactionFormData(string transactionId);

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> GetOwnerId(string accountId);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist
		/// and ArgumentException when the User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionDetailsServiceModel> GetTransactionDetails(
			string transactionId, string ownerId, bool isUserAdmin);

		Task<UsersAccountCardsServiceModel> GetAccountCardsData(int page);

		Task<IEnumerable<CurrencyCashFlowServiceModel>> GetCurrenciesCashFlow();

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<bool> IsAccountDeleted(string accountId);

		/// <summary>
		/// Throws ArgumentNullException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<bool> IsAccountOwner(string userId, string accountId);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDetailsShortServiceModel> GetAccountShortDetails(string accountId);
	}
}
