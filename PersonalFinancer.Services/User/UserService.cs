﻿using PersonalFinancer.Web.Data;

namespace PersonalFinancer.Services.User
{
	public class UserService : IUserService
	{
		private readonly PersonalFinancerDbContext data;

		public UserService(PersonalFinancerDbContext context)
		{
			this.data = context;
		}

		public async Task<string?> FullName(string userId)
		{
			var user = await data.Users.FindAsync(userId);

			if (user == null) 
				return null;

			return $"{user.FirstName} {user.LastName}";
		}
	}
}
