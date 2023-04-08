namespace PersonalFinancer.Web.Models.Accounts
{
	using Web.Models.Shared;
	
	public class AccountCardExtendedViewModel : AccountCardViewModel
	{
		public string OwnerId { get; set; } = null!;
	}
}
