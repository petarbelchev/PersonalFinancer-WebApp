using PersonalFinancer.Services.ApiService.Models;
using System.ComponentModel.DataAnnotations;
using static PersonalFinancer.Data.Constants.CategoryConstants;

namespace PersonalFinancer.Services.Shared.Models
{
	public class CategoryInputModel : IApiInputServiceModel
	{
		[Required(ErrorMessage = "Please enter a Category name.")]
		[StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength,
			ErrorMessage = "Category name must be between {2} and {1} characters long.")]
		public string Name { get; init; } = null!;

		[Required(ErrorMessage = "Owner Id is required!")]
		public string OwnerId { get; set; } = null!;
	}
}
