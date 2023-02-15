namespace PersonalFinancer.Services.Account.Models
{
	public class AccountDetailsViewModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string Currency { get; set; } = null!;

		public IEnumerable<TransactionExtendedViewModel> Transactions { get; set; }
			= new List<TransactionExtendedViewModel>();
	}
}
