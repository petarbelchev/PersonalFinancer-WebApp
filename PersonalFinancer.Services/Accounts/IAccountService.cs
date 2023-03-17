using PersonalFinancer.Services.Accounts.Models;

namespace PersonalFinancer.Services.Accounts
{
	public interface IAccountService
	{
		int GetUsersAccountsCount();

		Task<IEnumerable<AccountDropdownViewModel>> GetUserAccountsDropdownViewModel(string userId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDropdownViewModel> GetAccountDropdownViewModel(string accountId);

		Task<AllUsersAccountCardsViewModel> GetAllUsersAccountCardsViewModel(int page);

		Task<IEnumerable<AccountCardViewModel>> GetUserAccountCardsViewModel(string userId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<DetailsAccountViewModel> GetAccountDetailsViewModel(string accountId, DateTime startDate, DateTime endDate, int page = 1);
		
		/// <summary>
		/// Throws ArgumentException when User already have Account with given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		Task<string> CreateAccount(string userId, AccountFormModel accountModel);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccount(string accountId, string userId, bool transactionsDelete);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<DeleteAccountViewModel> GetDeleteAccountViewModel(string accountId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditAccount(string accountId, AccountFormModel model, string userId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountFormModel> GetEditAccountModel(string accountId);
		
		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> GetOwnerId(string accountId);
		
		/// <summary>
		/// Throws ArgumentNullException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<bool> IsAccountOwner(string userId, string accountId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<bool> IsAccountDeleted(string accountId);

		Task<Dictionary<string, CashFlowViewModel>> GetUserAccountsCashFlow(
			string userId, DateTime? startDate, DateTime? endDate);

		Task<Dictionary<string, CashFlowViewModel>> GetAllAccountsCashFlow();
	}
}
