namespace PersonalFinancer.Services.User.Models
{
	using Services.Shared.Models;

	public class UserDetailsServiceModel : UserServiceModel
	{
		public string PhoneNumber { get; set; } = null!;

		public IEnumerable<AccountCardServiceModel> Accounts { get; set; }
			= new List<AccountCardServiceModel>();
	}
}
