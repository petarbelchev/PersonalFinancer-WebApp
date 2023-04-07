using PersonalFinancer.Web.Models.Shared;

namespace PersonalFinancer.Web.Models.Accounts
{
	public class TransactionsViewModel
	{
        public IEnumerable<TransactionTableViewModel> Transactions { get; set; } = null!;

        public PaginationModel Pagination { get; set; } = new PaginationModel
        {
            ElementsName = "transactions"
        };

        public string TransactionDetailsUrl { get; set; } = null!;
    }
}
