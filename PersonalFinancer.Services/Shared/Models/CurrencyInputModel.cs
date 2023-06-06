using PersonalFinancer.Services.ApiService.Models;
using System.ComponentModel.DataAnnotations;
using static PersonalFinancer.Data.Constants.CurrencyConstants;

namespace PersonalFinancer.Services.Shared.Models
{
	public class CurrencyInputModel : IApiInputServiceModel
	{
		[Required(ErrorMessage = "Please enter a Currency name.")]
		[StringLength(CurrencyNameMaxLength, MinimumLength = CurrencyNameMinLength,
			ErrorMessage = "Currency name must be between {2} and {1} characters long.")]
		public string Name { get; init; } = null!;

		[Required(ErrorMessage = "Owner Id is required!")]
		public string OwnerId { get; set; } = null!;
	}
}
