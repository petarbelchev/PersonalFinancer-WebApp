using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts
{
    public interface IAccountsService
	{
		/// <summary>
		/// Throws ArgumentException when User already have Account with given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		Task<string> CreateAccount(AccountFormModel model);
		
		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> CreateTransaction(string userId, TransactionFormModel transactionFormModel);
					
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
		Task EditAccount(string accountId, AccountFormModel model);
				
		/// <summary>
		/// Throws InvalidOperationException when Transaction or Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditTransaction(string id, TransactionFormModel editedTransaction);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<DetailsAccountViewModel> GetAccountDetailsViewModel(string accountId, DateTime startDate, DateTime endDate, int page = 1, string? ownerId = null);
	
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDropdownViewModel> GetAccountDropdownViewModel(string accountId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist 
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountFormModel> GetAccountFormModel(string accountId, string? ownerId = null);

		Task<Dictionary<string, CashFlowViewModel>> GetUsersAccountsCashFlow();

		Task<UsersAccountCardsViewModel> GetUsersAccountCardsViewModel(int page);
		
		Task SetUserTransactionsViewModel(string userId, UserTransactionsViewModel model);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<DeleteAccountViewModel> GetDeleteAccountViewModel(string accountId, string? ownerId = null);
		
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountFormModel> GetEmptyAccountFormModel(string userId);
		
		Task<TransactionFormModel> GetEmptyTransactionFormModel(string userId);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionFormModel> GetFulfilledTransactionFormModel(string transactionId);
		
		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> GetOwnerId(string accountId);
		
		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TransactionDetailsViewModel> GetTransactionViewModel(string transactionId);

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
		Task PrepareAccountDetailsViewModelForReturn(string accountId, DetailsAccountViewModel model);
		
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task PrepareAccountFormModelForReturn(AccountFormModel model);
	}
}
