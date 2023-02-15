namespace PersonalFinancer.Services.Account.Models
{
    public class DashboardViewModel
	{
		public IEnumerable<TransactionShortViewModel> LastTransactions { get; set; }
			= new List<TransactionShortViewModel>();

		public IEnumerable<AccountCardViewModel> Accounts { get; set; }
			= new List<AccountCardViewModel>();

		public Dictionary<string, CashFlowViewModel> CurrenciesCashFlow { get; set; }
			= new Dictionary<string, CashFlowViewModel>();
	}
}
