namespace PersonalFinancer.Services.Account.Models
{
	public class AccountViewModelExtended
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string? Currency { get; set; }

		public IEnumerable<TransactionViewModel>? Transactions { get; set; }
	}
}
