namespace PersonalFinancer.Web.Models.Account
{
	using System.ComponentModel.DataAnnotations;

	using static Data.DataConstants.User;

	public class LoginFormModel
	{
		[Required]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; } = null!;

		[Required]
		[DataType(DataType.Password)]
		[StringLength(UserPasswordMaxLength, MinimumLength = UserPasswordMinLength,
			ErrorMessage = "Password must be between {2} and {1} characters long.")]
		public string Password { get; set; } = null!;

		public bool RememberMe { get; set; }

		public string? ReturnUrl { get; set; }
	}
}
