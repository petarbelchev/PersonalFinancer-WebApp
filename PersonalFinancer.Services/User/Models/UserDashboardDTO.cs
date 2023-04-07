using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.User.Models
{
    public class UserDashboardDTO
	{
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<TransactionTableDTO> Transactions { get; set; } = null!;

        public IEnumerable<AccountCardDTO> Accounts { get; set; } = null!;

        public IEnumerable<CurrencyCashFlowDTO> CurrenciesCashFlow { get; set; } = null!;
	}
}
