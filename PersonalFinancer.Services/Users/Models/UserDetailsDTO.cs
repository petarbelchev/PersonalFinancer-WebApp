namespace PersonalFinancer.Services.Users.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class UserDetailsDTO : UserInfoDTO
	{
		public string PhoneNumber { get; set; } = null!;

		public IEnumerable<AccountCardDTO> Accounts { get; set; }
			= new List<AccountCardDTO>();
	}
}
