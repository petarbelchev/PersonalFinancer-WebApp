namespace PersonalFinancer.Services.Categories.Models
{
	public class CategoryInputDTO
    {
        public string Name { get; init; } = null!;

        public string OwnerId { get; set; } = null!;
    }
}
