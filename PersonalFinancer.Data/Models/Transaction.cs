namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	
	using Data.Enums;
	using static Data.Constants.TransactionConstants;

	public class Transaction
	{
		[Key]
		public Guid Id { get; set; }

		[ForeignKey(nameof(Account))]
		public Guid AccountId { get; set; }
		public Account Account { get; set; } = null!;

		public decimal Amount { get; set; }

		[ForeignKey(nameof(Category))]
		public Guid CategoryId { get; set; }
		public Category Category { get; set; } = null!;

		public TransactionType TransactionType { get; set; }

		[DataType(DataType.DateTime, ErrorMessage = "Plaese enter a valid Date.")]
		public DateTime CreatedOn { get; set; }

		[MaxLength(TransactionRefferenceMaxLength, ErrorMessage = "Refference max length must be {1} characters long.")]
		public string Refference { get; set; } = null!;
	}
}
