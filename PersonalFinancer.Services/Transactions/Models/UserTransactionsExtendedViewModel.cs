using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Transactions.Models
{
	public class UserTransactionsExtendedViewModel
	{
        public DateFilterModel Dates { get; set; } = null!;

        public PaginationModel Pagination { get; set; } 
			= new PaginationModel();

        public IEnumerable<TransactionExtendedViewModel> Transactions { get; set; }
			= new List<TransactionExtendedViewModel>();
	}
}
