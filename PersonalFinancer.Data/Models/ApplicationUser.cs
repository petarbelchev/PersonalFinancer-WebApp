using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static PersonalFinancer.Data.DataConstants.User;

namespace PersonalFinancer.Data.Models
{
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
