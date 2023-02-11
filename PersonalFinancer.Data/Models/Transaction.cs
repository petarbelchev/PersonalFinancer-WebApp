namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	
	using Data.Enums;
	using static Data.DataConstants.Transaction;

	public class Transaction
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey(nameof(Account))]
		public int AccountId { get; set; }
		public Account Account { get; set; } = null!;

		public decimal Amount { get; set; }

		[ForeignKey(nameof(Category))]
		public int CategoryId { get; set; }
		public Category Category { get; set; } = null!;

		public TransactionType TransactionType { get; set; }

		public DateTime CreatedOn { get; set; }

		[Required]
		[MaxLength(TransactionRefferenceMaxLength,
			ErrorMessage = "Refference max length must be {1} characters long.")]
		public string Refference { get; set; } = null!;
	}
}
