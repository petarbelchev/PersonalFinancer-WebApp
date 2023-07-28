namespace PersonalFinancer.Services.Users.Models
{
	public class UsersInfoDTO
	{
		public IEnumerable<UserInfoDTO> Users { get; set; } = null!;

		public int TotalUsersCount { get; set; }
	}
}
