namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	using static Data.Constants.CurrencyConstants;

	public class Currency : ApiEntity
	{
		[Key]
		public override string Id { get; set; } = null!;

		[MaxLength(CurrencyNameMaxLength,
			ErrorMessage = "Currency's name max length must be {1} characters long.")]
		public override string Name { get; set; } = null!;

		[ForeignKey(nameof(Owner))]
		public override string OwnerId { get; set; } = null!;
		public ApplicationUser Owner { get; set; } = null!;

		public override bool IsDeleted { get; set; }

		public ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
	}
}
