namespace PersonalFinancer.Services.User.Models
{
	public class UsersServiceModel
	{
		public IEnumerable<UserServiceModel> Users { get; set; } = null!;

        public int TotalUsersCount { get; set; }
    }
}
