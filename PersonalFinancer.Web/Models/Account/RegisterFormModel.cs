namespace PersonalFinancer.Web.Models.Account
{
	using System.ComponentModel.DataAnnotations;
	
	using static Data.DataConstants.User;

	public class RegisterFormModel
	{
		[Required]
		[StringLength(UserFirstNameMaxLength, MinimumLength = UserFirstNameMinLength,
			ErrorMessage = "First name must be between {2} and {1} characters long.")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; } = null!;

		[Required]
		[StringLength(UserLastNameMaxLength, MinimumLength = UserLastNameMinLength,
			ErrorMessage = "Last name must be between {2} and {1} characters long.")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; } = null!;

		[Required]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; } = null!;

		[Required]
		[DataType(DataType.Password)]
		[StringLength(UserPasswordMaxLength, MinimumLength = UserPasswordMinLength,
			ErrorMessage = "Password must be between {2} and {1} characters long.")]
		public string Password { get; set; } = null!;

		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Password do not match.")]
		[Display(Name = "Confirm Password")]
		public string ConfirmPassword { get; set; } = null!;

		public string? ReturnUrl { get; set; }
	}
}