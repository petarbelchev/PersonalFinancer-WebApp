﻿namespace PersonalFinancer.Services.Users
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.Users.Models;

	public interface IUsersService
	{
		Task<IEnumerable<string>> GetAdminsIdsAsync();

		Task<AccountsAndCategoriesDropdownDTO> GetUserAccountsAndCategoriesDropdownsAsync(Guid userId);

		Task<IEnumerable<AccountCardDTO>> GetUserAccountsCardsAsync(Guid userId);

		Task<AccountTypesAndCurrenciesDropdownDTO> GetUserAccountTypesAndCurrenciesDropdownsAsync(Guid userId);

		Task<UserDashboardDTO> GetUserDashboardDataAsync(Guid userId, DateTime fromLocalTime, DateTime toLocalTime);

		Task<UserUsedDropdownsDTO> GetUserUsedDropdownsAsync(Guid userId);

		Task<UsersInfoDTO> GetUsersInfoAsync(int page, string? search = null);

		Task<TransactionsDTO> GetUserTransactionsAsync(TransactionsFilterDTO dto);

		/// <exception cref="InvalidOperationException">When the user does not exist.</exception>
		Task<UserDetailsDTO> UserDetailsAsync(Guid userId);

		/// <exception cref="InvalidOperationException">When the user does not exist.</exception>
		Task<string> UserFullNameAsync(Guid userId);

		Task<int> UsersCountAsync();
	}
}
