using System.ComponentModel.DataAnnotations;

using static PersonalFinancer.Data.Constants.AccountTypeConstants;

namespace PersonalFinancer.Services.AccountTypes.Models
{
	public class AccountTypeViewModel
	{
		public string Id { get; set; } = null!;

		[StringLength(AccountTypeNameMaxLength, MinimumLength = AccountTypeNameMinLength,
			ErrorMessage = "Account Type name must be between {2} and {1} characters long.")]
		public string Name { get; init; } = null!;

		public string? UserId { get; set; }
	}
}
