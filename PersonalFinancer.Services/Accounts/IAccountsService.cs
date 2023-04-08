namespace PersonalFinancer.Services.Accounts
{
	using Services.Accounts.Models;
	using Services.Shared.Models;

	public interface IAccountsService
	{
		/// <summary>
		/// Throws ArgumentException when User already have Account with given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		Task<string> CreateAccount(CreateAccountFormDTO model);

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> CreateTransaction(CreateTransactionInputDTO inputDTO);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccount(string accountId, bool shouldDeleteTransactions = false, string? userId = null);

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist
		/// and ArgumentException when Owner Id is passed and User is not owner.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<decimal> DeleteTransaction(string transactionId, string? ownerId = null);

		/// <summary>
		/// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditAccount(EditAccountFormDTO model);

		/// <summary>
		/// Throws InvalidOperationException when Transaction or Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditTransaction(EditTransactionInputDTO inputDTO);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDetailsOutputDTO> GetAccountDetails(
			AccountDetailsInputDTO inputDTO, string? ownerId = null);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDTO> GetAccount(string accountId);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist 
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<EditAccountFormDTO> GetAccountForm(string accountId, string? ownerId = null);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist or Start date is after End date.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountTransactionsOutputDTO> GetAccountTransactions(AccountTransactionsInputDTO inputDTO);

		Task<AccountCardsOutputDTO> GetUsersAccountCards(int page, int elementsPerPage);

		Task<IEnumerable<CurrencyCashFlowDTO>> GetUsersCurrenciesCashFlow();

		/// <summary>
		/// Throws InvalidOperationException when User does not exist or Start date is after End date.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<UserTransactionsApiOutputDTO> GetUserTransactionsApi(UserTransactionsApiInputDTO inputDTO);

		Task<UserTransactionsOutputDTO> GetUserTransactions(UserTransactionsInputDTO input);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<DeleteAccountDTO> GetDeleteAccountData(string accountId, string? ownerId = null);

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<CreateAccountFormDTO> GetEmptyAccountForm(string userId);

		Task<EmptyTransactionFormDTO> GetEmptyTransactionForm(string userId);

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<FulfilledTransactionFormDTO> GetFulfilledTransactionForm(string transactionId);

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> GetOwnerId(string accountId);

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionDetailsDTO> GetTransactionDetails(string transactionId);

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
		Task<AccountDetailsOutputDTO> GetAccountDetailsForReturn(AccountDetailsInputDTO inputDTO);
	}
}
