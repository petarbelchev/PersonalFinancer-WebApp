using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.User
{
	public interface IUsersService
	{
		
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> FullName(string userId);

		Task<AllUsersDTO> GetAllUsers(int page, int elementsPerPage);

		Task<IEnumerable<AccountCardDTO>> GetUserAccounts(string userId);
				
		int GetUsersAccountsCount();

		Task<UserDashboardDTO> GetUserDashboardData(string userId, DateTime startDate, DateTime endDate);
		
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<UserDetailsDTO> UserDetails(string userId);

		int UsersCount();
	}
}
