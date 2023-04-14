namespace PersonalFinancer.Data.Models
{
	using Microsoft.AspNetCore.Identity;
	using System.ComponentModel.DataAnnotations;

	using static Data.Constants.UserConstants;

	public class ApplicationUser : IdentityUser
	{
		[MaxLength(UserFirstNameMaxLength,
			ErrorMessage = "First name max length must be {1} characters long.")]
		public string FirstName { get; set; } = null!;

		[MaxLength(UserLastNameMaxLength,
			ErrorMessage = "Last name max length must be {1} characters long.")]
		public string LastName { get; set; } = null!;

		public ICollection<Account> Accounts { get; set; } = new HashSet<Account>();

		public ICollection<AccountType> AccountTypes { get; set; } = new HashSet<AccountType>();

		public ICollection<Category> Categories { get; set; } = new HashSet<Category>();

		public ICollection<Currency> Currencies { get; set; } = new HashSet<Currency>();

		public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
	}
}
