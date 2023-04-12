namespace PersonalFinancer.Services.User.Models
{
	using Services.Shared.Models;

	public class UserDetailsServiceModel : UserServiceModel
	{
		public IEnumerable<AccountCardServiceModel> Accounts { get; set; }
			= new List<AccountCardServiceModel>();
	}
}
