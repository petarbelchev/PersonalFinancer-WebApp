namespace PersonalFinancer.Services.User
{
	public interface IUserService
	{
		/// <summary>
		/// Returns User's full name or null when user does not exist.
		/// </summary>
		Task<string?> FullName(string userId);
	}
}
