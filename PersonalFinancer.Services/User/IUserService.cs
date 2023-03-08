using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.User
{
	public interface IUserService
	{
		/// <summary>
		/// Returns collection of all users 
		/// </summary>
		Task<IEnumerable<UserViewModel>> All();

		/// <summary>
		/// Returns Dashboard View Model for current User with Last transactions, Accounts and Currencies Cash Flow.
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		/// <exception cref="ArgumentException"></exception>
		Task GetUserDashboard(string userId, DashboardServiceModel model);

		/// <summary>
		/// Returns count of all registered users.
		/// </summary>
		int UsersCount();

		/// <summary>
		/// Returns User Details View Model used for Admin User Details page.
		/// Throws Exception if the User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<UserDetailsViewModel> UserDetails(string userId);

		/// <summary>
		/// Returns User's full name or throws exception when user does not exist.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task<string> FullName(string userId);
	}
}
