using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.User
{
    public interface IUserService
	{
		Task<AllUsersViewModel> GetAllUsers(int page = 1);

		/// <summary>
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task GetUserDashboard(string userId, HomeIndexViewModel model);

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
	}
}
