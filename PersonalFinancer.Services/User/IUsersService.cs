namespace PersonalFinancer.Services.User
{
	using Services.Shared.Models;
	using Services.User.Models;

	public interface IUsersService
	{
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> FullName(string userId);

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
