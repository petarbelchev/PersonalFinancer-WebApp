using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
    public class UsersAccountsCardsServiceModel
	{
		public IEnumerable<AccountCardServiceModel> Accounts { get; set; } = null!;

        public int TotalUsersAccountsCount { get; set; }
    }
}
