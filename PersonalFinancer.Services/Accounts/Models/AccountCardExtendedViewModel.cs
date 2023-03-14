using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountCardExtendedViewModel : AccountCardViewModel
	{
        public string OwnerId { get; set; } = null!;
    }
}
