using PersonalFinancer.Web.Models.Shared;

namespace PersonalFinancer.Web.Models.Accounts
{
    public class AccountCardExtendedViewModel : AccountCardViewModel
	{
        public string OwnerId { get; set; } = null!;
    }
}
