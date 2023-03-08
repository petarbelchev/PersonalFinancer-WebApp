using System.ComponentModel.DataAnnotations;

using static PersonalFinancer.Data.Constants.UserConstants;

namespace PersonalFinancer.Web.Models.Account
{
	public class RegisterFormModel
	{
		[Required(ErrorMessage = "First Name is required.")]
		[StringLength(UserFirstNameMaxLength, MinimumLength = UserFirstNameMinLength,
			ErrorMessage = "First name must be between {2} and {1} characters long.")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; } = null!;

		[Required(ErrorMessage = "Last Name is required.")]
		[StringLength(UserLastNameMaxLength, MinimumLength = UserLastNameMinLength,
			ErrorMessage = "Last name must be between {2} and {1} characters long.")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; } = null!;

		[Required(ErrorMessage = "Email address is required.")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		[StringLength(UserPasswordMaxLength, MinimumLength = UserPasswordMinLength,
			ErrorMessage = "Password must be between {2} and {1} characters long.")]
		public string Password { get; set; } = null!;

		[Required(ErrorMessage = "Confirm Password is required.")]
		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Password do not match.")]
		[Display(Name = "Confirm Password")]
		public string ConfirmPassword { get; set; } = null!;
	}
}