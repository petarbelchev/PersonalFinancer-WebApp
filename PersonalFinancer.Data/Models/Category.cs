namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	using static Data.Constants.CategoryConstants;

	public class Category
	{
		[Key]
		public Guid Id { get; set; }

		[MaxLength(CategoryNameMaxLength, ErrorMessage = "Category's name max length must be {1} characters long.")]
		public string Name { get; set; } = null!;

		[ForeignKey(nameof(User))]
		public string? UserId { get; set; }
		public ApplicationUser? User { get; set; }

		public bool IsDeleted { get; set; }
	}
}
