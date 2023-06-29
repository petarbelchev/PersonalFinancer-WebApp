namespace PersonalFinancer.Services.User
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.User.Models;

	public interface IUsersService
	{
        Task<AccountsAndCategoriesDropdownDTO> GetUserAccountsAndCategoriesDropdownDataAsync(Guid userId);

        Task<AccountTypesAndCurrenciesDropdownDTO> GetUserAccountTypesAndCurrenciesDropdownDataAsync(Guid userId);
		
		Task<UserDashboardDTO> GetUserDashboardDataAsync(Guid userId, DateTime startDate, DateTime endDate);

        Task<UserDropdownDTO> GetUserDropdownDataAsync(Guid userId);
	
        Task<UsersInfoDTO> GetUsersInfoAsync(int page);

		Task<TransactionsDTO> GetUserTransactionsAsync(TransactionsFilterDTO dto);

		Task<TransactionsPageDTO> GetUserTransactionsPageDataAsync(TransactionsFilterDTO dto);

		/// <summary>
		/// Throws Invalid Operation Exception if the user does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<UserDetailsDTO> UserDetailsAsync(Guid userId);

		/// <summary>
		/// Throws Invalid Operation Exception if the user does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> UserFullNameAsync(Guid userId);

        Task<int> UsersCountAsync();
	}
}
