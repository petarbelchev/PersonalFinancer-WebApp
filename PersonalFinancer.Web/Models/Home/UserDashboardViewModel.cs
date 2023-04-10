namespace PersonalFinancer.Web.Models.Home
{
	using Web.Models.Shared;

	public class UserDashboardViewModel : DateFilterInputModel
	{
		public IEnumerable<TransactionTableViewModel> Transactions { get; set; }
			= new List<TransactionTableViewModel>();

		public IEnumerable<AccountCardViewModel> Accounts { get; set; }
			= new List<AccountCardViewModel>();

		public IEnumerable<CurrencyCashFlowViewModel> CurrenciesCashFlow { get; set; }
			= new List<CurrencyCashFlowViewModel>();
	}
}
