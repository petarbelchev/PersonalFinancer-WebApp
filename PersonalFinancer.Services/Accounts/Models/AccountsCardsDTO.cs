#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
    using PersonalFinancer.Services.Shared.Models;

    public class AccountsCardsDTO
	{
		public IEnumerable<AccountCardDTO> Accounts { get; set; }

        public int TotalAccountsCount { get; set; }
    }
}
