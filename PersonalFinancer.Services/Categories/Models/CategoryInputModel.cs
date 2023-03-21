using System.ComponentModel.DataAnnotations;

using static PersonalFinancer.Data.Constants.CategoryConstants;

namespace PersonalFinancer.Services.Categories.Models
{
	public class CategoryInputModel
	{
		[StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength,
			ErrorMessage = "Category name must be between {2} and {1} characters long.")]
		public string Name { get; init; } = null!;
	}
}
