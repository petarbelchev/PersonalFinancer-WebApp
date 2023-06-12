﻿namespace PersonalFinancer.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static PersonalFinancer.Data.Constants.AccountConstants;

    public class Account
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(AccountNameMaxLength,
           ErrorMessage = "Account name max length must be {1} characters long.")]
        public string Name { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }
        public ApplicationUser Owner { get; set; } = null!;

        [ForeignKey(nameof(AccountType))]
        public Guid AccountTypeId { get; set; }
        public AccountType AccountType { get; set; } = null!;

        [ForeignKey(nameof(Currency))]
        public Guid CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;

        public ICollection<Transaction> Transactions { get; set; }
           = new HashSet<Transaction>();

        public bool IsDeleted { get; set; }
    }
}
