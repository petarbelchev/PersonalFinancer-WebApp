namespace PersonalFinancer.Services.Accounts
{
	using PersonalFinancer.Services.Accounts.Models;

	public interface IAccountsUpdateService
	{
		/// <summary>
        /// Throws ArgumentException when User already have Account with the given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
        Task<Guid> CreateAccountAsync(CreateEditAccountDTO model);

        /// <summary>
        /// Throws InvalidOperationException if Account does not exist.
        /// </summary>
        /// <returns>New transaction Id.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task<Guid> CreateTransactionAsync(CreateEditTransactionDTO model);

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task DeleteAccountAsync(Guid accountId, Guid userId, bool isUserAdmin, bool shouldDeleteTransactions = false);

        /// <summary>
        /// Throws InvalidOperationException when Transaction does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task<decimal> DeleteTransactionAsync(Guid transactionId, Guid userId, bool isUserAdmin);

		/// <summary>
        /// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
        Task EditAccountAsync(Guid accountId, CreateEditAccountDTO model);

		/// <summary>
		/// Throws InvalidOperationException when Transaction, Category or Account does not exist
		/// or Transaction is initial.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditTransactionAsync(Guid transactionId, CreateEditTransactionDTO model);
	}
}
