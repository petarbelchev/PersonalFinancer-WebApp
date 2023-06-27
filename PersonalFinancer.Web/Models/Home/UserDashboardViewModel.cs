namespace PersonalFinancer.Web.Models.Home
{
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.Models.Shared;

    public class UserDashboardViewModel : DateFilterModel
    {
        public IEnumerable<TransactionTableDTO> Transactions { get; set; }
            = new List<TransactionTableDTO>();

        public IEnumerable<AccountCardDTO> Accounts { get; set; }
            = new List<AccountCardDTO>();

        public IEnumerable<CurrencyCashFlowWithExpensesByCategoriesDTO> CurrenciesCashFlow { get; set; }
            = new List<CurrencyCashFlowWithExpensesByCategoriesDTO>();
    }
}
