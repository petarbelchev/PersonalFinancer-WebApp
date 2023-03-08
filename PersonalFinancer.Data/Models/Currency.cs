using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static PersonalFinancer.Data.Constants.CurrencyConstants;

namespace PersonalFinancer.Data.Models
{
	public class Currency
	{
		[Key]
		public Guid Id { get; set; }

		[MaxLength(CurrencyNameMaxLength, ErrorMessage = "Currency's name max length must be {1} characters long.")]
		public string Name { get; set; } = null!;

		[ForeignKey(nameof(User))]
		public string? UserId { get; set; }
		public ApplicationUser? User { get; set; }

		public bool IsDeleted { get; set; }
	}
}
