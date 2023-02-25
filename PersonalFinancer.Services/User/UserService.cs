namespace PersonalFinancer.Services.User
{
	using Data;
	using Data.Models;

	public class UserService : IUserService
	{
		private readonly PersonalFinancerDbContext data;

		public UserService(PersonalFinancerDbContext context)
		{
			this.data = context;
		}

		/// <summary>
		/// Returns User's full name or throws exception when user does not exist.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<string> FullName(string userId)
		{
			ApplicationUser? user = await data.Users.FindAsync(userId);

			if (user == null)
			{
				throw new ArgumentNullException("User does not exist.");
			}

			return $"{user.FirstName} {user.LastName}";
		}
	}
}
