using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Transactions.Models;

namespace PersonalFinancer.Services.User.Models
{
	public class UserDashboardDTO
	{
		public IEnumerable<TransactionShortViewModel> LastTransactions { get; set; } = null!;

        public IEnumerable<AccountCardViewModel> Accounts { get; set; } = null!;

        public IEnumerable<TransactionServiceModel> CurrenciesCashFlow { get; set; } = null!;
	}
}
