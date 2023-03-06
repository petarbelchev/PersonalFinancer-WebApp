namespace PersonalFinancer.Services.Accounts
{
	using Models;

	public interface IAccountService
	{
		/// <summary>
		/// Returns count of all created accounts.
		/// </summary>
		int AccountsCount();

		/// <summary>
		/// Returns collection of User's accounts with Id and Name.
		/// </summary>
		Task<IEnumerable<AccountDropdownViewModel>> AllAccountsDropdownViewModel(string userId);

		/// <summary>
		/// Returns Account with Id and Name or null.
		/// </summary>
		Task<AccountDropdownViewModel?> AccountDropdownViewModel(Guid accountId);

		/// <summary>
		/// Returns a collection of user's accounts with Id, Name, Balance and Currency Name.
		/// </summary>
		Task<IEnumerable<AccountCardViewModel>> AllAccountsCardViewModel(string userId);

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
		Task<Guid> CreateAccount(string userId, AccountFormModel accountModel);
				
		/// <summary>
		/// Delete an Account and give the option to delete all of the account's transactions.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task DeleteAccountById(Guid accountId, string userId, bool transactionsDelete);

		/// <summary>
		/// Returns Delete Account View Model.
		/// </summary>
		Task<DeleteAccountViewModel?> DeleteAccountViewModel(Guid accountId);

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

		/// <summary>
		/// Returns User's accounts Cash Flow for a given period.
		/// </summary>
		Task<Dictionary<string, CashFlowViewModel>> GetUserAccountsCashFlow(
			string userId, DateTime? startDate, DateTime? endDate);

		/// <summary>
		/// Returns Cash Flow of all user's accounts.
		/// </summary>
		/// <returns></returns>
		Task<Dictionary<string, CashFlowViewModel>> GetAllAccountsCashFlow();
	}
}
