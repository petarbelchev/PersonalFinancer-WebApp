using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.Transactions.Models;

namespace PersonalFinancer.Services.User.Models
{
    public class HomeIndexViewModel
    {
        public DateFilterModel Dates { get; set; } = null!;

        public IEnumerable<TransactionShortViewModel> LastTransactions { get; set; }
            = new List<TransactionShortViewModel>();

        public IEnumerable<AccountCardViewModel> Accounts { get; set; }
            = new List<AccountCardViewModel>();

        public Dictionary<string, CashFlowViewModel> CurrenciesCashFlow { get; set; }
            = new Dictionary<string, CashFlowViewModel>();
    }
}
