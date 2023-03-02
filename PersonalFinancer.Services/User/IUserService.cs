using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.User
{
	public interface IUserService
	{
		Task<IEnumerable<UserViewModel>> All();

		Task<UserDetailsViewModel> UserDetails(string userId);

		/// <summary>
		/// Returns User's full name or throws exception when user does not exist.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task<string> FullName(string userId);
	}
}
