namespace PersonalFinancer.Services.Account.Models
{
	public class DashboardViewModel
	{
		public IEnumerable<TransactionViewModel> LastTransactions { get; set; }
			= new List<TransactionViewModel>();

		public IEnumerable<AccountViewModelExtended> Accounts { get; set; }
			= new List<AccountViewModelExtended>();
	}
}
