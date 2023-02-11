namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;

	public class Currency
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; } = null!;

		public string? UserId { get; set; }
	}
}
