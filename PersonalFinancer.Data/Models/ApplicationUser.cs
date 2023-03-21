using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

using static PersonalFinancer.Data.Constants.UserConstants;

namespace PersonalFinancer.Data.Models
{
	public class ApplicationUser : IdentityUser
	{
		[MaxLength(UserFirstNameMaxLength, 
			ErrorMessage = "First name max length must be {1} characters long.")]
		public string FirstName { get; set; } = null!;

		[MaxLength(UserLastNameMaxLength, 
			ErrorMessage = "Last name max length must be {1} characters long.")]
		public string LastName { get; set; } = null!;

		public ICollection<Account> Accounts { get; set; } = null!;

        public ICollection<AccountType> AccountTypes { get; set; } = null!;

        public ICollection<Category> Categories { get; set; } = null!;

        public ICollection<Currency> Currencies { get; set; } = null!;

        public ICollection<Transaction> Transactions { get; set; } = null!;
    }
}
