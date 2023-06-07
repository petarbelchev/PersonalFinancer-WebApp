using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.User
{
	public interface IUsersService
	{
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> UserFullName(string userId);

		Task<UsersServiceModel> GetAllUsers(int page);
		
		Task<UserAccountsAndCategoriesServiceModel> GetUserAccountsAndCategories(string userId);
		
		Task<UserAccountTypesAndCurrenciesServiceModel> GetUserAccountTypesAndCurrencies(string userId);

		Task<IEnumerable<AccountCardServiceModel>> GetUserAccounts(string userId);

		Task<int> GetUsersAccountsCount();
		
		Task<TransactionsServiceModel> GetUserTransactions(string userId, DateTime startDate, DateTime endDate, int page = 1);

		Task<UserDashboardServiceModel> GetUserDashboardData(string userId, DateTime startDate, DateTime endDate);

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<UserDetailsServiceModel> UserDetails(string userId);

		Task<int> UsersCount();
	}
}
