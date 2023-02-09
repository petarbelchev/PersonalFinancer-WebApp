using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Data.Models
{
	public class Currency
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; } = null!;

		public string? UserId { get; set; }
	}
}
