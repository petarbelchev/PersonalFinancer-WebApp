namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using static PersonalFinancer.Common.Constants.AccountConstants;

	public class Account
    {
		public Account() => this.Id = Guid.NewGuid();

        [Required]
		[Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(AccountNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required]
        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }
        public ApplicationUser Owner { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(AccountType))]
        public Guid AccountTypeId { get; set; }
        public AccountType AccountType { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Currency))]
        public Guid CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;

        public ICollection<Transaction> Transactions { get; set; }
           = new HashSet<Transaction>();

        [Required]
        public bool IsDeleted { get; set; }
    }
}
