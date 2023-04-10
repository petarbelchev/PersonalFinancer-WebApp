namespace PersonalFinancer.Web.Models.AccountTypes
{
	using System.ComponentModel.DataAnnotations;

	using static Data.Constants.AccountTypeConstants;

	public class AccountTypeInputModel
	{
		[StringLength(AccountTypeNameMaxLength, MinimumLength = AccountTypeNameMinLength,
			ErrorMessage = "Account Type name must be between {2} and {1} characters long.")]
		public string Name { get; init; } = null!;

		public string OwnerId { get; set; } = null!;
	}
}
