namespace PersonalFinancer.Web.Models.User
{
	using Web.Models.Shared;

	public class UserDetailsViewModel : UserViewModel
	{
		public IEnumerable<AccountCardViewModel> Accounts { get; set; } = null!;
	}
}
