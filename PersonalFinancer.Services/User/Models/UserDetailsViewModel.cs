using PersonalFinancer.Services.Accounts.Models;

namespace PersonalFinancer.Services.User.Models
{
	public class UserDetailsViewModel : UserViewModel
	{
		public IEnumerable<AccountCardViewModel> Accounts { get; set; }
			= new List<AccountCardViewModel>();
	}
}
