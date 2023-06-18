namespace PersonalFinancer.Services.Accounts
{
    using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using System;
	using System.Threading.Tasks;

    public interface IAccountsInfoService
    {
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDetailsServiceModel> GetAccountDetailsAsync(
			Guid id, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist 
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountFormServiceModel> GetAccountFormDataAsync(Guid accountId, Guid userId, bool isUserAdmin);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> GetAccountNameAsync(Guid accountId, Guid userId, bool isUserAdmin);

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionFormServiceModel> GetTransactionFormDataAsync(Guid transactionId);

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<Guid> GetOwnerIdAsync(Guid accountId);

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist
		/// and ArgumentException when the User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionDetailsServiceModel> GetTransactionDetailsAsync(
			Guid transactionId, Guid ownerId, bool isUserAdmin);

		Task<UsersAccountsCardsServiceModel> GetAccountsCardsDataAsync(int page);

		Task<int> GetAccountsCountAsync();

		Task<IEnumerable<CurrencyCashFlowServiceModel>> GetCashFlowByCurrenciesAsync();

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDetailsShortServiceModel> GetAccountShortDetailsAsync(Guid accountId);

		Task<TransactionsServiceModel> GetAccountTransactionsAsync(
			Guid accountId, DateTime startDate, DateTime endDate, int page = 1);

		Task<IEnumerable<AccountCardServiceModel>> GetUserAccountsAsync(Guid userId);

		Task<TransactionsServiceModel> GetUserTransactionsAsync(
			Guid userId, DateTime startDate, DateTime endDate, int page = 1);
	}
}
