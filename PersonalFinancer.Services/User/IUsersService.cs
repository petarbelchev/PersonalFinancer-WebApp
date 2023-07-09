namespace PersonalFinancer.Services.User
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.User.Models;

	public interface IUsersService
	{
		Task<IEnumerable<string>> GetAdminsIdsAsync();

		Task<AccountsAndCategoriesDropdownDTO> GetUserAccountsAndCategoriesDropdownDataAsync(Guid userId);

		Task<IEnumerable<AccountCardDTO>> GetUserAccountsCardsAsync(Guid userId);

		Task<AccountTypesAndCurrenciesDropdownDTO> GetUserAccountTypesAndCurrenciesDropdownDataAsync(Guid userId);
		
		Task<UserDashboardDTO> GetUserDashboardDataAsync(Guid userId, DateTime fromLocalTime, DateTime toLocalTime);

        Task<UserDropdownDTO> GetUserDropdownDataAsync(Guid userId);

        Task<UsersInfoDTO> GetUsersInfoAsync(int page);

		Task<TransactionsDTO> GetUserTransactionsAsync(TransactionsFilterDTO dto);

		Task<TransactionsPageDTO> GetUserTransactionsPageDataAsync(TransactionsFilterDTO dto);

		/// <exception cref="InvalidOperationException">When the user does not exist.</exception>
		Task<UserDetailsDTO> UserDetailsAsync(Guid userId);

		/// <exception cref="InvalidOperationException">When the user does not exist.</exception>
		Task<string> UserFullNameAsync(Guid userId);

        Task<int> UsersCountAsync();
	}
}
