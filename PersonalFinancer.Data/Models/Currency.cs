using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static PersonalFinancer.Data.Constants.AccountConstants;

namespace PersonalFinancer.Data.Models
{
	public class Currency
	{
		[Key]
		public string Id { get; set; } = null!;

		[MaxLength(CurrencyNameMaxLength, 
			ErrorMessage = "Currency's name max length must be {1} characters long.")]
		public string Name { get; set; } = null!;

		[ForeignKey(nameof(Owner))]
		public string OwnerId { get; set; } = null!;
		public ApplicationUser Owner { get; set; } = null!;

		public bool IsDeleted { get; set; }

        public ICollection<Account> Accounts { get; set; } = null!;
    }
}
