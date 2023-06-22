namespace PersonalFinancer.Data.Models
{
    using PersonalFinancer.Data.Models.Contracts;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static PersonalFinancer.Data.Constants.AccountTypeConstants;

    public class AccountType : CacheableApiEntity
    {
        [Key]
        public override Guid Id { get; set; }

        [MaxLength(AccountTypeNameMaxLength,
           ErrorMessage = "Account type name max length must be {1} characters long.")]
        public override string Name { get; set; } = null!;

        [ForeignKey(nameof(Owner))]
        public override Guid OwnerId { get; set; }

        public ApplicationUser Owner { get; set; } = null!;

        public override bool IsDeleted { get; set; }

        public ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
    }
}
