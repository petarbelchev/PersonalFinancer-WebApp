using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Transactions.Models
{
	public class UserTransactionsExtendedViewModel : PaginationModel
	{
		public IEnumerable<TransactionExtendedViewModel> Transactions { get; set; }
			= new List<TransactionExtendedViewModel>();
	}
}
