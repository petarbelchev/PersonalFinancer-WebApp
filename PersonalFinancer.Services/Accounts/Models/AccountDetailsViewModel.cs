using PersonalFinancer.Services.Shared.Models;
using static PersonalFinancer.Data.Constants.HostConstants;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsViewModel
	{
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public string OwnerId { get; set; } = null!;

		public IEnumerable<TransactionTableViewModel> Transactions { get; set; }
			= new List<TransactionTableViewModel>();

		public string ApiTransactionsEndpoint { get; set; }
			= ApiAccountTransactionsUrl;

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Controller = "Accounts",
			Action = "AccountDetails"
		};

		public PaginationModel Pagination { get; set; }
			= new PaginationModel();
	}
}
