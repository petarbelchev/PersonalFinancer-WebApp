#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	using PersonalFinancer.Data.Models.Enums;

	public class CreateEditTransactionInputDTO
	{
		public decimal Amount { get; set; }
		
		public Guid OwnerId { get; set; }

		public Guid CategoryId { get; set; }
		
		public Guid AccountId { get; set; }

		public DateTime CreatedOnLocalTime { get; set; }

		public string Reference { get; set; }

		public TransactionType TransactionType { get; set; }
	}
}
