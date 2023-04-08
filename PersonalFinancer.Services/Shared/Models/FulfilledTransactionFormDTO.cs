namespace PersonalFinancer.Services.Shared.Models
{
	using Data.Enums;

	public class FulfilledTransactionFormDTO : EmptyTransactionFormDTO
	{
		public decimal Amount { get; set; }

		public string CategoryId { get; set; } = null!;

		public string AccountId { get; set; } = null!;

		public string Refference { get; set; } = null!;

		public TransactionType TransactionType { get; set; }
	}
}
