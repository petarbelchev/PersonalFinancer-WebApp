using System.ComponentModel.DataAnnotations;

using static PersonalFinancer.Data.Constants.CategoryConstants;

namespace PersonalFinancer.Services.Categories.Models
{
	public class CategoryViewModel
	{
		public Guid Id { get; set; }

		[StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength,
			ErrorMessage = "Category name must be between {2} and {1} characters long.")]
		public string Name { get; init; } = null!;

		public string? UserId { get; set; }
	}
}
