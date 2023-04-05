using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class TransactionsViewModel
	{
        public IEnumerable<TransactionTableViewModel> Transactions { get; set; } = null!;

        public PaginationModel Pagination { get; set; }
            = new PaginationModel();

        public string TransactionDetailsUrl { get; set; } = null!;
    }
}
