using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using PersonalFinancer.Data.Enums;
using static PersonalFinancer.Data.Constants.TransactionConstants;

namespace PersonalFinancer.Data.Models
{
	public class Transaction
	{
		[Key]
		public string Id { get; set; } = null!;

		[ForeignKey(nameof(Account))]
		public string AccountId { get; set; } = null!;
		public Account Account { get; set; } = null!;

		public decimal Amount { get; set; }

		[ForeignKey(nameof(Category))]
		public string CategoryId { get; set; } = null!;
		public Category Category { get; set; } = null!;

		public TransactionType TransactionType { get; set; }

		[DataType(DataType.DateTime, ErrorMessage = "Plaese enter a valid Date.")]
		public DateTime CreatedOn { get; set; }

		[MaxLength(TransactionRefferenceMaxLength, ErrorMessage = "Refference max length must be {1} characters long.")]
		public string Refference { get; set; } = null!;
	}
}
