namespace PersonalFinancer.Web.Models.Account
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.ModelBinders;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.AccountConstants;

    public class AccountFormViewModel
	{
		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(AccountNameMaxLength, 
			MinimumLength = AccountNameMinLength,
			ErrorMessage = ValidationMessages.InvalidLength)]
		public string Name { get; set; } = null!;

		[Required]
		[ModelBinder(BinderType = typeof(DecimalModelBinder))]
		[Range(AccountInitialBalanceMinValue, 
			AccountInitialBalanceMaxValue,
			ErrorMessage = ValidationMessages.InvalidNumericLength)]
		public decimal Balance { get; set; }
		
		[Required]
		public Guid? OwnerId { get; set; }

		[Required]
		[Display(Name = "Account Type")]
		public Guid? AccountTypeId { get; set; }

		[Required]
		[Display(Name = "Currency")]
		public Guid? CurrencyId { get; set; }

		public IEnumerable<AccountTypeDropdownDTO> OwnerAccountTypes { get; set; }
			= new List<AccountTypeDropdownDTO>();

		public IEnumerable<CurrencyDropdownDTO> OwnerCurrencies { get; set; }
			= new List<CurrencyDropdownDTO>();
	}
}
