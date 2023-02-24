namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class Currency
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; } = null!;

		[ForeignKey(nameof(User))]
		public string? UserId { get; set; }
		public ApplicationUser? User { get; set; }
	}
}
