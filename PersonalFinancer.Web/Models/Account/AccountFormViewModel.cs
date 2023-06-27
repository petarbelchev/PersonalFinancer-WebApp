namespace PersonalFinancer.Web.Models.Account
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.ModelBinders;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.AccountConstants;

    public class AccountFormViewModel
	{
		[Required(ErrorMessage = "Account name is required.")]
		[StringLength(AccountNameMaxLength, MinimumLength = AccountNameMinLength,
			ErrorMessage = "Account name must be between {2} and {1} characters long.")]
		public string Name { get; set; } = null!;

		[ModelBinder(BinderType = typeof(DecimalModelBinder))]
		[Range(AccountInitialBalanceMinValue, AccountInitialBalanceMaxValue,
			ErrorMessage = "Balance must be a number between {1} and {2}")]
		public decimal Balance { get; set; }
		
		[Required(ErrorMessage = "Owner Id name is required.")]
		public Guid? OwnerId { get; set; }

		[Required(ErrorMessage = "Account Type name is required.")]
		[Display(Name = "Account Type")]
		public Guid? AccountTypeId { get; set; }

		public IEnumerable<AccountTypeDropdownDTO> OwnerAccountTypes { get; set; }
			= new List<AccountTypeDropdownDTO>();

		[Required(ErrorMessage = "Currency name is required.")]
		[Display(Name = "Currency")]
		public Guid? CurrencyId { get; set; }

		public IEnumerable<CurrencyDropdownDTO> OwnerCurrencies { get; set; }
			= new List<CurrencyDropdownDTO>();
	}
}
