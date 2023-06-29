namespace PersonalFinancer.Data.Models
{
    using PersonalFinancer.Data.Models.Contracts;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static PersonalFinancer.Common.Constants.AccountTypeConstants;

    public class AccountType : BaseApiEntity
    {
        [Required]
        [Key]
        public override Guid Id { get; set; }

        [Required]
        [MaxLength(AccountTypeNameMaxLength)]
        public override string Name { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Owner))]
        public override Guid OwnerId { get; set; }

        public ApplicationUser Owner { get; set; } = null!;

        [Required]
        public override bool IsDeleted { get; set; }

        public ICollection<Account> Accounts { get; set; } 
            = new HashSet<Account>();
    }
}
