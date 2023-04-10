using PersonalFinancer.Services.Shared.Models;
using static PersonalFinancer.Data.Constants.HostConstants;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class UserTransactionsViewModel
    {
        public string Id { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<TransactionTableViewModel> Transactions { get; set; }
            = new List<TransactionTableViewModel>();
        
		public string ApiTransactionsEndpoint { get; set; }
			= ApiTransactionsUrl;

        public RoutingModel Routing { get; set; } = new RoutingModel
        {
            Controller = "Transactions",
            Action = "All"
        };

        public PaginationModel Pagination { get; set; }
            = new PaginationModel();
    }
}
