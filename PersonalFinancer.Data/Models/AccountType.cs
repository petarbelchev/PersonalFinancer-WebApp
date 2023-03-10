using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static PersonalFinancer.Data.Constants.AccountTypeConstants;

namespace PersonalFinancer.Data.Models
{
	public class AccountType
	{
		[Key]
		public Guid Id { get; set; }

		[MaxLength(AccountTypeNameMaxLength, ErrorMessage = "Account type name max length must be {1} characters long.")]
		public string Name { get; set; } = null!;

		[ForeignKey(nameof(User))]
		public string? UserId { get; set; }
		public ApplicationUser? User { get; set; }

		public bool IsDeleted { get; set; }
	}
}
