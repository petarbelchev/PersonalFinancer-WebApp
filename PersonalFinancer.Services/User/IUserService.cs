namespace PersonalFinancer.Services.User
{
	public interface IUserService
	{
		/// <summary>
		/// Returns User's full name or throws exception when user does not exist.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task<string> FullName(string userId);
	}
}
