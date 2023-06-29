namespace PersonalFinancer.Data.Models
{
    using PersonalFinancer.Data.Models.Contracts;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static PersonalFinancer.Common.Constants.CategoryConstants;

    public class Category : BaseApiEntity
    {
        [Required]
        [Key]
        public override Guid Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public override string Name { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Owner))]
        public override Guid OwnerId { get; set; }
        public ApplicationUser Owner { get; set; } = null!;

        [Required]
        public override bool IsDeleted { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
           = new HashSet<Transaction>();
    }
}
