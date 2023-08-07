namespace PersonalFinancer.Services.Accounts
{
    using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using System;
	using System.Threading.Tasks;

    public interface IAccountsInfoService
	{
		/// <exception cref="UnauthorizedAccessException">When user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		Task<AccountDetailsDTO> GetAccountDetailsAsync(Guid accountId, Guid userId, bool isUserAdmin);

		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		Task<CreateEditAccountOutputDTO> GetAccountFormDataAsync(Guid accountId, Guid userId, bool isUserAdmin);

		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		Task<string> GetAccountNameAsync(Guid accountId, Guid userId, bool isUserAdmin);

		Task<AccountsCardsDTO> GetAccountsCardsDataAsync(int page, string? search);

		Task<int> GetAccountsCountAsync();

		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		Task<TransactionsDTO> GetAccountTransactionsAsync(AccountTransactionsFilterDTO dto);

		Task<IEnumerable<CurrencyCashFlowDTO>> GetCashFlowByCurrenciesAsync();

		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When the transaction does not exist.</exception>
		Task<TransactionDetailsDTO> GetTransactionDetailsAsync(Guid transactionId, Guid ownerId, bool isUserAdmin);

		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When the transaction does not exist.</exception>
		Task<CreateEditTransactionOutputDTO> GetTransactionFormDataAsync(Guid transactionId, Guid userId, bool isUserAdmin);
	}
}
