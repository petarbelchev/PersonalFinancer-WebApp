namespace PersonalFinancer.Services.User.Models
{
    using PersonalFinancer.Services.Shared.Models;

    public class UserDashboardServiceModel
    {
        public IEnumerable<TransactionTableServiceModel> LastTransactions { get; set; }
            = new List<TransactionTableServiceModel>();

        public IEnumerable<AccountCardServiceModel> Accounts { get; set; }
            = new List<AccountCardServiceModel>();

        public IEnumerable<CurrencyCashFlowServiceModel> CurrenciesCashFlow { get; set; }
            = new List<CurrencyCashFlowServiceModel>();
    }
}
