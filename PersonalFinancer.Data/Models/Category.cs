namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	using static Data.Constants.CategoryConstants;

	public class Category : ApiEntity
	{
		[Key]
		public override string Id { get; set; } = null!;

		[MaxLength(CategoryNameMaxLength,
			ErrorMessage = "Category's name max length must be {1} characters long.")]
		public override string Name { get; set; } = null!;

		[ForeignKey(nameof(Owner))]
		public override string OwnerId { get; set; } = null!;
		public ApplicationUser Owner { get; set; } = null!;

		public override bool IsDeleted { get; set; }

		public ICollection<Transaction> Transactions { get; set; } 
			= new HashSet<Transaction>();
	}
}
