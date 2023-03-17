using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Transactions.Models
{
	public class UserTransactionsExtendedViewModel
	{
        public DateFilterModel Dates { get; set; } = null!;

        public PaginationModel Pagination { get; set; } = new PaginationModel();

        public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Controller = "Transaction",
			Action = "All"
		};

        public IEnumerable<TransactionExtendedViewModel> Transactions { get; set; }
			= new List<TransactionExtendedViewModel>();
	}
}
