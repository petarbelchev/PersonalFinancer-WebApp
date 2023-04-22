namespace PersonalFinancer.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using PersonalFinancer.Data.Contracts;
    using static Data.Constants.AccountTypeConstants;

    public class AccountType : ApiEntity
	{
		[Key]
		public override string Id { get; set; } = null!;

		[MaxLength(AccountTypeNameMaxLength,
			ErrorMessage = "Account type name max length must be {1} characters long.")]
		public override string Name { get; set; } = null!;

		[ForeignKey(nameof(Owner))]
		public override string OwnerId { get; set; } = null!;

		public ApplicationUser Owner { get; set; } = null!;

		public override bool IsDeleted { get; set; }

		public ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
	}
}
