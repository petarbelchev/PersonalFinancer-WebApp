using System.ComponentModel.DataAnnotations;

using static PersonalFinancer.Data.Constants.CurrencyConstants;

namespace PersonalFinancer.Services.Currencies.Models
{
	public class CurrencyViewModel
	{
		public Guid Id { get; init; }

		[StringLength(CurrencyNameMaxLength, MinimumLength = CurrencyNameMinLength,
			ErrorMessage = "Currency name must be between {2} and {1} characters long.")]
		public string Name { get; init; } = null!;

		public string? UserId { get; set; }
	}
}
