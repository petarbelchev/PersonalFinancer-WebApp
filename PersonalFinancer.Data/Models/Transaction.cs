namespace PersonalFinancer.Data.Models
{
    using PersonalFinancer.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static PersonalFinancer.Data.Constants.TransactionConstants;

    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [ForeignKey(nameof(Account))]
        public Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }
        public ApplicationUser Owner { get; set; } = null!;

        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
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
