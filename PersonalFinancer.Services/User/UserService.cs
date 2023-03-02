namespace PersonalFinancer.Services.User
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;

	using Models;
	using Data;
	using Data.Models;
	using Microsoft.EntityFrameworkCore;

	public class UserService : IUserService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;

		public UserService(
			PersonalFinancerDbContext context,
			IMapper mapper)
		{
			this.data = context;
			this.mapper = mapper;
		}

		public async Task<IEnumerable<UserViewModel>> All()
		{
			IEnumerable<UserViewModel> users = await data.Users
				.ProjectTo<UserViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			return users;
		}

		public async Task<UserDetailsViewModel> UserDetails(string userId)
		{
			UserDetailsViewModel user = await data.Users
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsViewModel>(mapper.ConfigurationProvider)
				.FirstAsync();

			return user;
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
