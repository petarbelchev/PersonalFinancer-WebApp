namespace PersonalFinancer.Services.Category.Models
{
	public class CategoryViewModel
	{
		public Guid Id { get; init; }
		
		public string Name { get; init; } = null!;

		public string? UserId { get; set; }
	}
}
