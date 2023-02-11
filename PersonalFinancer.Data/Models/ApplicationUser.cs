namespace PersonalFinancer.Data.Models
{
	using Microsoft.AspNetCore.Identity;
	using System.ComponentModel.DataAnnotations;
	
	using static Data.DataConstants.User;

	public class ApplicationUser : IdentityUser
	{
		[Required]
		[MaxLength(UserFirstNameMaxLength,
			ErrorMessage = "First name max length must be {1} characters long.")]
		public string FirstName { get; set; } = null!;

		[Required]
		[MaxLength(UserLastNameMaxLength,
			ErrorMessage = "Last name max length must be {1} characters long.")]
		public string LastName { get; set; } = null!;
	}
}
