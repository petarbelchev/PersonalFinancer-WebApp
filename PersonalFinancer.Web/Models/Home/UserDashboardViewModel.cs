namespace PersonalFinancer.Web.Models.Home
{
	using Services.Shared.Models;
	using Services.User.Models;

    using Web.Models.Shared;

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
