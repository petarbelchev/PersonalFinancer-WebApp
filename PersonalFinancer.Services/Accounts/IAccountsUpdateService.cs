namespace PersonalFinancer.Services.Accounts
{
	using PersonalFinancer.Services.Accounts.Models;

	public interface IAccountsUpdateService
	{
		/// <summary>
		/// Throws Argument Exception when the user already have account with the given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		Task<Guid> CreateAccountAsync(CreateEditAccountDTO model);

		/// <summary>
		/// Throws Invalid Operation Exception if the account does not exist.
		/// </summary>
		/// <returns>New transaction Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		Task<Guid> CreateTransactionAsync(CreateEditTransactionDTO model);

		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist
		/// and Argument Exception when the user is not owner or administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccountAsync(Guid accountId, Guid userId, bool isUserAdmin, bool shouldDeleteTransactions = false);

		/// <summary>
		/// Throws Invalid Operation Exception when the transaction does not exist
		/// and Argument Exception when the user is not owner or administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<decimal> DeleteTransactionAsync(Guid transactionId, Guid userId, bool isUserAdmin);

		/// <summary>
		/// Throws Invalid Operation Exception when the account does now exist,
		/// and Argument Exception when the user already have account with the given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditAccountAsync(Guid accountId, CreateEditAccountDTO model);

		/// <summary>
		/// Throws Invalid Operation Exception when the transaction, category or account does not exist
		/// or the transaction is initial.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditTransactionAsync(Guid transactionId, CreateEditTransactionDTO model);
	}
}
