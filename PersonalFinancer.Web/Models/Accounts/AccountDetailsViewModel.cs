namespace PersonalFinancer.Web.Models.Accounts
{
	using Web.Models.Shared;

	using static Data.Constants.HostConstants;

	public class AccountDetailsViewModel : AccountCardExtendedViewModel
	{
		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public IEnumerable<TransactionTableViewModel> Transactions { get; set; } = null!;

		public string ApiTransactionsEndpoint { get; set; }
			= ApiAccountTransactionsUrl;

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Controller = "Accounts",
			Action = "AccountDetails"
		};

		public PaginationModel Pagination { get; set; } = new PaginationModel
		{
			ElementsName = "transactions"
		};
	}
}
