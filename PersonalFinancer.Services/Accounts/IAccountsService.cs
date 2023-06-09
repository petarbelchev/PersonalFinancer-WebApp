namespace PersonalFinancer.Services.Accounts
{
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Shared.Models;

    public interface IAccountsService
    {
        /// <summary>
        /// Throws ArgumentException when User already have Account with the given name.
        /// </summary>
        /// <returns>New Account Id.</returns>
        /// <exception cref="ArgumentException"></exception>
        Task<Guid> CreateAccount(AccountFormShortServiceModel model);

        /// <summary>
        /// Throws InvalidOperationException if Account does not exist.
        /// </summary>
        /// <returns>New transaction Id.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task<Guid> CreateTransaction(TransactionFormShortServiceModel model);

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task DeleteAccount(Guid? accountId, Guid userId, bool isUserAdmin, bool shouldDeleteTransactions = false);

        /// <summary>
        /// Throws InvalidOperationException when Transaction does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task<decimal> DeleteTransaction(Guid? transactionId, Guid userId, bool isUserAdmin);

        /// <summary>
        /// Throws InvalidOperationException when Account does now exist,
        /// and ArgumentException when User already have Account with given name.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task EditAccount(Guid? accountId, AccountFormShortServiceModel model);

        /// <summary>
        /// Throws InvalidOperationException when Transaction or Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task EditTransaction(Guid id, TransactionFormShortServiceModel model);

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist
        /// or User is not owner or Administrator.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<AccountDetailsServiceModel> GetAccountDetails(
            Guid? id, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin);

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist 
        /// or User is not owner or Administrator.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<AccountFormServiceModel> GetAccountFormData(Guid? accountId, Guid userId, bool isUserAdmin);

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<TransactionsServiceModel> GetAccountTransactions(
            Guid? id, DateTime startDate, DateTime endDate, int page);

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist
        /// or User is not owner or Administrator.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<string> GetAccountName(Guid? accountId, Guid userId, bool isUserAdmin);

        /// <summary>
        /// Throws InvalidOperationException when Transaction does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<TransactionFormServiceModel> GetTransactionFormData(Guid transactionId);

        /// <summary>
        /// Throws InvalidOperationException if Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<Guid> GetOwnerId(Guid? accountId);

        /// <summary>
        /// Throws InvalidOperationException when Transaction does not exist
        /// and ArgumentException when the User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task<TransactionDetailsServiceModel> GetTransactionDetails(
            Guid transactionId, Guid ownerId, bool isUserAdmin);

        Task<UsersAccountsCardsServiceModel> GetAccountsCardsData(int page);

        Task<IEnumerable<CurrencyCashFlowServiceModel>> GetCurrenciesCashFlow();

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<bool> IsAccountDeleted(Guid accountId);

        /// <summary>
        /// Throws ArgumentNullException when Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<bool> IsAccountOwner(Guid userId, Guid accountId);

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<AccountDetailsShortServiceModel> GetAccountShortDetails(Guid? accountId);
    }
}
