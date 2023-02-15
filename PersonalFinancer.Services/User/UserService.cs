namespace PersonalFinancer.Services.User
{
	using Data;

	public class UserService : IUserService
	{
		private readonly PersonalFinancerDbContext data;

		public UserService(PersonalFinancerDbContext context)
		{
			this.data = context;
		}

		/// <summary>
		/// Returns User's full name or null when user does not exist.
		/// </summary>
		public async Task<string?> FullName(string userId)
		{
			var user = await data.Users.FindAsync(userId);

			if (user == null)
			{
				return null;
			}

			return $"{user.FirstName} {user.LastName}";
		}
	}
}
