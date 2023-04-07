using PersonalFinancer.Web.Models.Shared;

namespace PersonalFinancer.Web.Models.User
{
	public class UserDetailsViewModel : UserViewModel
	{
		public IEnumerable<AccountCardViewModel> Accounts { get; set; } = null!;
	}
}
