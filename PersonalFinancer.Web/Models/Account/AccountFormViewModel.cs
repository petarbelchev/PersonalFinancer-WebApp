namespace PersonalFinancer.Web.Models.Account
{
	using Microsoft.AspNetCore.Mvc;

	using System.ComponentModel.DataAnnotations;

	using Services.Shared.Models;
	
	using Web.ModelBinders;
	
	using static Data.Constants.AccountConstants;

	public class AccountFormViewModel
	{
		[Required(ErrorMessage = "Account name is required.")]
		[StringLength(AccountNameMaxLength, MinimumLength = AccountNameMinLength,
			ErrorMessage = "Account name must be between {2} and {1} characters long.")]
		public string Name { get; set; } = null!;

		[ModelBinder(BinderType = typeof(DecimalModelBinder))]
		[Range(AccountInitialBalanceMinValue, AccountInitialBalanceMaxValue,
			ErrorMessage = "Ballace must be a number between {1} and {2}")]
		public decimal Balance { get; set; }

		public string OwnerId { get; set; } = null!;

		[Required(ErrorMessage = "Account Type name is required.")]
		[Display(Name = "Account Type")]
		public string AccountTypeId { get; set; } = null!;

		public IEnumerable<AccountTypeServiceModel> AccountTypes { get; set; }
			= new List<AccountTypeServiceModel>();

		[Required(ErrorMessage = "Currency name is required.")]
		[Display(Name = "Currency")]
		public string CurrencyId { get; set; } = null!;

		public IEnumerable<CurrencyServiceModel> Currencies { get; set; }
			= new List<CurrencyServiceModel>();
	}
}
