namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	using Enums;
	using static Constants.TransactionConstants;

	public class Transaction
	{
		[Key]
		public string Id { get; set; } = null!;

		[Column(TypeName = "decimal(18,2)")]
		public decimal Amount { get; set; }

		[ForeignKey(nameof(Account))]
		public string AccountId { get; set; } = null!;
		public Account Account { get; set; } = null!;

		[ForeignKey(nameof(Owner))]
		public string OwnerId { get; set; } = null!;
		public ApplicationUser Owner { get; set; } = null!;

		[ForeignKey(nameof(Category))]
		public string CategoryId { get; set; } = null!;
		public Category Category { get; set; } = null!;

		public TransactionType TransactionType { get; set; }

		[DataType(DataType.DateTime, ErrorMessage = "Plaese enter a valid Date.")]
		public DateTime CreatedOn { get; set; }

		[MaxLength(TransactionRefferenceMaxLength,
			ErrorMessage = "Refference max length must be {1} characters long.")]
		public string Refference { get; set; } = null!;

		public bool IsInitialBalance { get; set; }
	}
}
