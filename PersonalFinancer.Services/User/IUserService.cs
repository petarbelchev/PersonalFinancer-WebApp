using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.User
{
    public interface IUserService
	{
		Task<AllUsersViewModel> GetAllUsers(int page = 1);

		Task SetUserDashboard(string userId, UserDashboardViewModel model);

		Task<IEnumerable<AccountCardViewModel>> GetUserAccounts(string userId);

		int UsersCount();
		
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<UserDetailsViewModel> UserDetails(string userId);
		
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> FullName(string userId);
				
		int GetUsersAccountsCount();
	}
}
