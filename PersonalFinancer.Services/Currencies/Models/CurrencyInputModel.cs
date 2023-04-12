namespace PersonalFinancer.Services.Currencies.Models
{
	using System.ComponentModel.DataAnnotations;

	using static Data.Constants.AccountConstants;
	
	public class CurrencyInputModel
	{
		[StringLength(CurrencyNameMaxLength, MinimumLength = CurrencyNameMinLength,
			ErrorMessage = "Currency name must be between {2} and {1} characters long.")]
		public string Name { get; init; } = null!;

		public string OwnerId { get; set; } = null!;
	}
}
