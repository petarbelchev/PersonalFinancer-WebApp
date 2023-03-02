namespace PersonalFinancer.Data.Models
{
	using Microsoft.AspNetCore.Identity;
	using System.ComponentModel.DataAnnotations;
	
	using static Data.Constants.UserConstants;

	public class ApplicationUser : IdentityUser
	{
		[MaxLength(UserFirstNameMaxLength, ErrorMessage = "First name max length must be {1} characters long.")]
		public string FirstName { get; set; } = null!;

		[MaxLength(UserLastNameMaxLength, ErrorMessage = "Last name max length must be {1} characters long.")]
		public string LastName { get; set; } = null!;

		public IEnumerable<Account> Accounts { get; set; }
			= new List<Account>();
	}
}
