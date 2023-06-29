namespace PersonalFinancer.Data.Models
{
    using PersonalFinancer.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static PersonalFinancer.Common.Constants.TransactionConstants;

    public class Transaction
    {
		public Transaction() => this.Id = Guid.NewGuid();

        [Required]
		[Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [ForeignKey(nameof(Account))]
        public Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }
        public ApplicationUser Owner { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [MaxLength(TransactionReferenceMaxLength)]
        public string Reference { get; set; } = null!;

        [Required]
        public bool IsInitialBalance { get; set; }
    }
}
