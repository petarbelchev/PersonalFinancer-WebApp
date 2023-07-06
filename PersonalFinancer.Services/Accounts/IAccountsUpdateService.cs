namespace PersonalFinancer.Services.Accounts
{
	using PersonalFinancer.Services.Accounts.Models;

	public interface IAccountsUpdateService
	{
		/// <exception cref="ArgumentException">When the user already have account with the given name.</exception>
		Task<Guid> CreateAccountAsync(CreateEditAccountDTO model);

		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		Task<Guid> CreateTransactionAsync(CreateEditTransactionDTO model);

		/// <exception cref="ArgumentException">When the user is not owner or administrator.</exception>
		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		Task DeleteAccountAsync(Guid accountId, Guid userId, bool isUserAdmin, bool shouldDeleteTransactions = false);

		/// <exception cref="ArgumentException">When the user is not owner or administrator.</exception>
		/// <exception cref="InvalidOperationException">When the transaction does not exist.</exception>
		Task<decimal> DeleteTransactionAsync(Guid transactionId, Guid userId, bool isUserAdmin);

		/// <exception cref="ArgumentException">When the user already have account with the given name.</exception>
		/// <exception cref="InvalidOperationException">When the account does now exist.</exception>
		Task EditAccountAsync(Guid accountId, CreateEditAccountDTO model);

		/// <exception cref="InvalidOperationException">When the transaction, category, or account does not exist, or when the transaction is initial.</exception>
		Task EditTransactionAsync(Guid transactionId, CreateEditTransactionDTO model);
	}
}
