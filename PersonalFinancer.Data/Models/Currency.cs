namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;

	public class Currency
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; } = null!;

		public string? UserId { get; set; }
	}
}
