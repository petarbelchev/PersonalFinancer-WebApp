using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static PersonalFinancer.Data.Constants.AccountConstants;

namespace PersonalFinancer.Data.Models
{
	public class Account
	{
		[Key]
		public string Id { get; set; } = null!;

		[MaxLength(AccountNameMaxLength, ErrorMessage = "Account name max length must be {1} characters long.")]
		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		[ForeignKey(nameof(Owner))]
		public string OwnerId { get; set; } = null!;
		public ApplicationUser Owner { get; set; } = null!;

		[ForeignKey(nameof(AccountType))]
		public string AccountTypeId { get; set; } = null!;
		public AccountType AccountType { get; set; } = null!;

		[ForeignKey(nameof(Currency))]
		public string CurrencyId { get; set; } = null!;
		public Currency Currency { get; set; } = null!;

		public ICollection<Transaction> Transactions { get; set; } = null!;

		public bool IsDeleted { get; set; }
	}
}
