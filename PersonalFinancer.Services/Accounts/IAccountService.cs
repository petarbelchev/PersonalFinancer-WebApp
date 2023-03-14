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
		Task<AccountDropdownViewModel> GetAccountDropdownViewModel(Guid accountId);

		Task<AllUsersAccountCardsViewModel> GetAllUsersAccountCardsViewModel(int page);

		Task<IEnumerable<AccountCardViewModel>> GetUserAccountCardsViewModel(string userId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountDetailsViewModel> GetAccountDetailsViewModel(Guid accountId, DateTime startDate, DateTime endDate, int page = 1);
		
		/// <summary>
		/// Throws ArgumentException when User already have Account with given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		Task<Guid> CreateAccount(string userId, CreateAccountFormModel accountModel);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccount(Guid accountId, string userId, bool transactionsDelete);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<DeleteAccountViewModel> GetDeleteAccountViewModel(Guid accountId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task EditAccount(EditAccountFormModel accountModel, string userId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<EditAccountFormModel> GetEditAccountFormModel(Guid accountId);
		
		/// <summary>
		/// Throws ArgumentNullException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<bool> IsAccountOwner(string userId, Guid accountId);
		
		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<bool> IsAccountDeleted(Guid accountId);

		Task<Dictionary<string, CashFlowViewModel>> GetUserAccountsCashFlow(
			string userId, DateTime? startDate, DateTime? endDate);

		Task<Dictionary<string, CashFlowViewModel>> GetAllAccountsCashFlow();
	}
}
