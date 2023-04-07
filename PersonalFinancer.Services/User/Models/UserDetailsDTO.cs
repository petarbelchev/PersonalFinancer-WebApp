using PersonalFinancer.Services.Accounts.Models;

namespace PersonalFinancer.Services.User.Models
{
	public class UserDetailsDTO : UserDTO
	{
		public IEnumerable<AccountCardDTO> Accounts { get; set; } = null!;
	}
}
