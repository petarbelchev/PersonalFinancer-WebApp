using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.User.Models
{
    public class UserDashboardViewModel : DateFilterModel
    {
        public IEnumerable<TransactionTableViewModel> Transactions { get; set; }
            = new List<TransactionTableViewModel>();

        public IEnumerable<AccountCardViewModel> Accounts { get; set; }
            = new List<AccountCardViewModel>();

        public IEnumerable<CurrencyCashFlowViewModel> CurrenciesCashFlow { get; set; }
            = new List<CurrencyCashFlowViewModel>();
    }
}
