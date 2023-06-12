namespace PersonalFinancer.Web.Models.Home
{
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.Models.Shared;

    public class UserDashboardViewModel : DateFilterModel
    {
        public IEnumerable<TransactionTableServiceModel> Transactions { get; set; }
            = new List<TransactionTableServiceModel>();

        public IEnumerable<AccountCardServiceModel> Accounts { get; set; }
            = new List<AccountCardServiceModel>();

        public IEnumerable<CurrencyCashFlowServiceModel> CurrenciesCashFlow { get; set; }
            = new List<CurrencyCashFlowServiceModel>();
    }
}
