using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.User
{
    public interface IUserService
	{
		
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> FullName(string userId);

		Task<AllUsersViewModel> GetAllUsers(int page = 1);

		Task<IEnumerable<AccountCardViewModel>> GetUserAccounts(string userId);
				
		int GetUsersAccountsCount();

		Task SetUserDashboard(string userId, UserDashboardViewModel model);
		
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<UserDetailsViewModel> UserDetails(string userId);

		int UsersCount();
	}
}
