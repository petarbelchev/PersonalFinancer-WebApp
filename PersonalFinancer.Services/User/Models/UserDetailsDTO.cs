namespace PersonalFinancer.Services.User.Models
{
	using Services.Accounts.Models;
	
	public class UserDetailsDTO : UserDTO
	{
		public IEnumerable<AccountCardDTO> Accounts { get; set; } = null!;
	}
}
