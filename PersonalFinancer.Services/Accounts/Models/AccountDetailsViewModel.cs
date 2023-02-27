namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsViewModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;

		public IEnumerable<AccountDetailsTransactionViewModel> Transactions { get; set; }
			= new List<AccountDetailsTransactionViewModel>();
	}
}
