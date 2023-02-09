using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static PersonalFinancer.Data.DataConstants.Account;

namespace PersonalFinancer.Data.Models
{
	public class Account
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(AccountNameMaxLength,
			ErrorMessage = "Account name max length must be {1} characters long.")]
		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		[ForeignKey(nameof(Owner))]
		public string OwnerId { get; set; } = null!;
		public ApplicationUser Owner { get; set; } = null!;

		[Required]
		[ForeignKey(nameof(AccountType))]
		public int AccountTypeId { get; set; }
		public AccountType AccountType { get; set; } = null!;

		[ForeignKey(nameof(Currency))]
		public int CurrencyId { get; set; }
		public Currency Currency { get; set; } = null!;

		public ICollection<Transaction> Transactions { get; set; } = null!;
	}
}
