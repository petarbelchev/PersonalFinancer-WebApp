namespace PersonalFinancer.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Common.Constants.UserConstants;

    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(UserFirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(UserLastNameMaxLength)]
        public string LastName { get; set; } = null!;

        [Required]
        public bool IsAdmin { get; set; }

        public ICollection<Account> Accounts { get; set; } 
            = new HashSet<Account>();

        public ICollection<AccountType> AccountTypes { get; set; } 
            = new HashSet<AccountType>();

        public ICollection<Category> Categories { get; set; } 
            = new HashSet<Category>();

        public ICollection<Currency> Currencies { get; set; } 
            = new HashSet<Currency>();

        public ICollection<Transaction> Transactions { get; set; } 
            = new HashSet<Transaction>();
    }
}
