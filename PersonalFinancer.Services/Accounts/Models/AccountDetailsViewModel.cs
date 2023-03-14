using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsViewModel
	{
		public string? Name { get; set; }

		public decimal Balance { get; set; }

		public string? CurrencyName { get; set; }

        public DateFilterModel Dates { get; set; } = null!;

        public PaginationModel Pagination { get; set; } 
			= new PaginationModel();

        public IEnumerable<AccountDetailsTransactionViewModel> Transactions { get; set; }
			= new List<AccountDetailsTransactionViewModel>();
	}
}
