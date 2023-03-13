using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsViewModel : PaginationModel
	{
		public string? Name { get; set; }

		public decimal Balance { get; set; }

		public string? CurrencyName { get; set; }

		public IEnumerable<AccountDetailsTransactionViewModel> Transactions { get; set; }
			= new List<AccountDetailsTransactionViewModel>();
	}
}
