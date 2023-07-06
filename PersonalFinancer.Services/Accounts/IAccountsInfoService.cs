﻿namespace PersonalFinancer.Services.Accounts
{
    using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using System;
	using System.Threading.Tasks;

    public interface IAccountsInfoService
	{
		/// <exception cref="InvalidOperationException">When the account does not exist or the user is not owner or administrator.</exception>
		Task<AccountDetailsLongDTO> GetAccountDetailsAsync(Guid accountId, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin);

		/// <exception cref="InvalidOperationException">When the account does not exist or the user is not owner or administrator.</exception>
		Task<CreateEditAccountDTO> GetAccountFormDataAsync(Guid accountId, Guid userId, bool isUserAdmin);

		/// <exception cref="InvalidOperationException">When the account does not exist or the user is not owner or administrator.</exception>
		Task<string> GetAccountNameAsync(Guid accountId, Guid userId, bool isUserAdmin);

		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		Task<Guid> GetAccountOwnerIdAsync(Guid accountId);

		Task<AccountsCardsDTO> GetAccountsCardsDataAsync(int page);

		Task<int> GetAccountsCountAsync();

		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		Task<AccountDetailsShortDTO> GetAccountShortDetailsAsync(Guid accountId);

		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		Task<TransactionsDTO> GetAccountTransactionsAsync(AccountTransactionsFilterDTO dto);

		Task<IEnumerable<CurrencyCashFlowDTO>> GetCashFlowByCurrenciesAsync();

		/// <exception cref="ArgumentException">When the user is not owner or administrator.</exception>
		/// <exception cref="InvalidOperationException">When the transaction does not exist.</exception>
		Task<TransactionDetailsDTO> GetTransactionDetailsAsync(Guid transactionId, Guid ownerId, bool isUserAdmin);

		/// <exception cref="InvalidOperationException">When the user is not owner, transaction does not exist or is initial.</exception>
		Task<CreateEditTransactionDTO> GetTransactionFormDataAsync(Guid transactionId, Guid userId, bool isUserAdmin);

		/// <exception cref="InvalidOperationException">When the transaction does not exist.</exception>
		Task<Guid> GetTransactionOwnerIdAsync(Guid transactionId);

		Task<IEnumerable<AccountCardDTO>> GetUserAccountsCardsAsync(Guid userId);
	}
}
