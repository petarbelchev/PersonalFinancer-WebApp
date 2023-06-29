namespace PersonalFinancer.Services.Accounts
{
    using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using System;
	using System.Threading.Tasks;

    public interface IAccountsInfoService
	{
		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist
		/// or the user is not owner or administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDetailsLongDTO> GetAccountDetailsAsync(
			Guid accountId, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin);

		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist 
		/// or the user is not owner or administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<CreateEditAccountDTO> GetAccountFormDataAsync(Guid accountId, Guid userId, bool isUserAdmin);

		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist
		/// or the user is not owner or administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> GetAccountNameAsync(Guid accountId, Guid userId, bool isUserAdmin);

		/// <summary>
		/// Throws Invalid Operation Exception if the account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<Guid> GetAccountOwnerIdAsync(Guid accountId);

		Task<AccountsCardsDTO> GetAccountsCardsDataAsync(int page);

		Task<int> GetAccountsCountAsync();

		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDetailsShortDTO> GetAccountShortDetailsAsync(Guid accountId);

		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionsDTO> GetAccountTransactionsAsync(AccountTransactionsFilterDTO dto);

		Task<IEnumerable<CurrencyCashFlowDTO>> GetCashFlowByCurrenciesAsync();

		/// <summary>
		/// Throws Invalid Operation Exception when the transaction does not exist
		/// and Argument Exception when the user is not owner or administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionDetailsDTO> GetTransactionDetailsAsync(
			Guid transactionId, Guid ownerId, bool isUserAdmin);

		/// <summary>
		/// Throws Invalid Operation Exception if the transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<Guid> GetTransactionOwnerIdAsync(Guid transactionId);

		/// <summary>
		/// Throws Invalid Operation Exception when the user is not owner, 
		/// transaction does not exist or is initial.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<CreateEditTransactionDTO> GetTransactionFormDataAsync(Guid transactionId, Guid userId, bool isUserAdmin);

		Task<IEnumerable<AccountCardDTO>> GetUserAccountsCardsAsync(Guid userId);
	}
}
