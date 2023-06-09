namespace PersonalFinancer.Services.Accounts.Models
{
    using PersonalFinancer.Data.Models.Enums;

    public class TransactionFormShortServiceModel
	{
		public decimal Amount { get; set; }

		public Guid OwnerId { get; set; }

		public Guid CategoryId { get; set; }

		public Guid AccountId { get; set; }

		public DateTime CreatedOn { get; set; }

		public string Refference { get; set; } = null!;

		public TransactionType TransactionType { get; set; }

		public bool IsInitialBalance { get; set; }
	}
}
