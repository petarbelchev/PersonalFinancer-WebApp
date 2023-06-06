using PersonalFinancer.Services.ApiService.Models;
using System.ComponentModel.DataAnnotations;
using static PersonalFinancer.Data.Constants.AccountTypeConstants;

namespace PersonalFinancer.Services.Shared.Models
{
	public class AccountTypeInputModel : IApiInputServiceModel
	{
		[Required(ErrorMessage = "Please enter an Account Type name.")]
		[StringLength(AccountTypeNameMaxLength, MinimumLength = AccountTypeNameMinLength,
			ErrorMessage = "Account Type name must be between {2} and {1} characters long.")]
		public string Name { get; init; } = null!;

		[Required(ErrorMessage = "Owner Id is required!")]
		public string OwnerId { get; set; } = null!;
	}
}
