namespace PersonalFinancer.Services.User.Models
{
	using Data.Enums;

	public class TransactionDTO
	{
		public string CurrencyName { get; set; } = null!;

		public TransactionType TransactionType { get; set; }

		public decimal Amount { get; set; }
	}
}
