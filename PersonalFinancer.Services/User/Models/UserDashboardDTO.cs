namespace PersonalFinancer.Services.User.Models
{
    using PersonalFinancer.Services.Shared.Models;

    public class UserDashboardDTO
    {
        public IEnumerable<TransactionTableDTO> LastTransactions { get; set; }
            = new List<TransactionTableDTO>();

        public IEnumerable<AccountCardDTO> Accounts { get; set; }
            = new List<AccountCardDTO>();

        public IEnumerable<CurrencyCashFlowWithExpensesByCategoriesDTO> CurrenciesCashFlow { get; set; }
            = new List<CurrencyCashFlowWithExpensesByCategoriesDTO>();
    }
}
