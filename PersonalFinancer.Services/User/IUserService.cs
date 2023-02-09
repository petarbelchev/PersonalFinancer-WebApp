namespace PersonalFinancer.Services.User
{
	public interface IUserService
	{
		Task<string?> FullName(string userId);
	}
}
