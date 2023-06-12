namespace PersonalFinancer.Services.Accounts.Models
{
    using PersonalFinancer.Services.Shared.Models;

    public class UsersAccountsCardsServiceModel
	{
		public IEnumerable<AccountCardServiceModel> Accounts { get; set; } = null!;

        public int TotalUsersAccountsCount { get; set; }
    }
}
