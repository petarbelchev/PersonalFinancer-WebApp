using System.ComponentModel.DataAnnotations;

using static PersonalFinancer.Data.Constants.UserConstants;

namespace PersonalFinancer.Web.Models.User
{
	public class LoginFormViewModel
	{
		[Required(ErrorMessage = "Email address is required.")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		[StringLength(UserPasswordMaxLength, MinimumLength = UserPasswordMinLength,
			ErrorMessage = "Password must be between {2} and {1} characters long.")]
		public string Password { get; set; } = null!;

		public bool RememberMe { get; set; }
	}
}
