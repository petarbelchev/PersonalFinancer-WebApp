namespace PersonalFinancer.Services.Account
{
	using Models;
	using Data.Enums;

	public interface IAccountService
	{
		/// <summary>
		/// Returns collection of User's accounts with Id and Name.
		/// </summary>
		Task<IEnumerable<AccountDropdownViewModel>> AllAccountsDropdownViewModel(string userId);

		/// <summary>
		/// Returns Account with Id and Name or null.
		/// </summary>
		Task<AccountDropdownViewModel?> AccountDropdownViewModel(Guid accountId);

		/// <summary>
		/// Returns collection of Account Types for the current user with Id and Name.
		/// </summary>
		Task<IEnumerable<AccountTypeViewModel>> AccountTypesViewModel(string userId);


		/// <summary>
		/// Returns Account with Id, Name, Balance and all Transactions or null.
		/// </summary>
		Task<AccountDetailsViewModel?> AccountDetailsViewModel(Guid accountId);

		/// <summary>
		/// Creates a new Account and if the new account has initial balance creates new Transaction with given amount.
		/// Returns new Account's id.
		/// </summary>
		/// <param name="userId">User's identifier</param>
		/// <param name="accountModel">Model with Name, Balance, AccountTypeId, CurrencyId.</param>
		Task<Guid> CreateAccount(string userId, AccountFormModel accountModel);

		/// <summary>
		/// Changes balance on given Account or throws exception if Account does not exist.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task ChangeBalance(Guid accountId, decimal amount, TransactionType transactionType);		

		/// <summary>
		/// Creates a new Transaction and change account's balance. Returns new transaction's id.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task<Guid> CreateTransaction(TransactionFormModel transactionFormModel);

		/// <summary>
		/// Delete a Transaction and change account's balance. Returns True or False.
		/// </summary>
		Task<bool> DeleteTransactionById(Guid transactionId);
		
		/// <summary>
		/// Delete an Account and give the option to delete all of the account's transactions.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task DeleteAccountById(Guid accountId, bool transactionsDelete);

		/// <summary>
		/// Returns Dashboard View Model for current User with Last transactions, Accounts and Currencies Cash Flow.
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		/// <exception cref="ArgumentException"></exception>
		Task DashboardViewModel(string userId, DashboardServiceModel model);

		/// <summary>
		/// Edits a Transaction and change account's balance if it's nessesery, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task EditTransaction(EditTransactionFormModel transactionFormModel);

		/// <summary>
		/// Returns a Transaction with Id, AccountId, Amount, CategoryId, Refference, TransactionType, OwnerId, CreatedOn, or null.
		/// </summary>
		Task<EditTransactionFormModel?> EditTransactionFormModelById(Guid transactionId);

		/// <summary>
		/// Returns Transaction Extended View Model with given Id, or null.
		/// </summary>
		Task<TransactionExtendedViewModel?> TransactionViewModel(Guid transactionId);

		/// <summary>
		/// Returns a collection of User's transactions for given period.
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		/// <exception cref="ArgumentException"></exception>
		Task<AllTransactionsServiceModel> AllTransactionsViewModel(string userId, AllTransactionsServiceModel model);

		/// <summary>
		/// Checks is the given User is owner of the given account, if does not exist, throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task<bool> IsAccountOwner(string userId, Guid accountId);

		/// <summary>
		/// Checks is the given Account deleted, if does not exist, throws an exception.
		/// </summary>
		/// <param name="accountId"></param>
		/// <exception cref="ArgumentNullException"></exception>
		Task<bool> IsAccountDeleted(Guid accountId);
	}
}
