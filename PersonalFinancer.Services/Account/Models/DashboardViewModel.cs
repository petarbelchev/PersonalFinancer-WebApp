namespace PersonalFinancer.Services.Account.Models
{
    public class DashboardViewModel
	{
		public IEnumerable<TransactionViewModel> LastTransactions { get; set; }
			= new List<TransactionViewModel>();

		public IEnumerable<AccountViewModelExtended> Accounts { get; set; }
			= new List<AccountViewModelExtended>();

		public Dictionary<string, CashFlowViewModel> CurrenciesCashFlow { get; set; }
			= new Dictionary<string, CashFlowViewModel>();
	}
}
