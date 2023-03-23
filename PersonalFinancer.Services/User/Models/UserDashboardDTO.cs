using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.User.Models
{
    public class UserDashboardDTO
	{
		public IEnumerable<TransactionTableViewModel> LastTransactions { get; set; } = null!;

        public IEnumerable<AccountCardViewModel> Accounts { get; set; } = null!;

        public IEnumerable<TransactionDTO> CurrenciesCashFlow { get; set; } = null!;
	}
}
