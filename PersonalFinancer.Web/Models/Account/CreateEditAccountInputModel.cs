namespace PersonalFinancer.Web.Models.Account
{
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Web.CustomModelBinders;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.AccountConstants;
	using static PersonalFinancer.Common.Messages.ValidationMessages;

	public class CreateEditAccountInputModel
	{
		[Required(ErrorMessage = RequiredProperty)]
		[StringLength(AccountNameMaxLength,
					  MinimumLength = AccountNameMinLength,
					  ErrorMessage = InvalidLength)]
		public string Name { get; set; } = null!;

		[Required(ErrorMessage = RequiredProperty)]
		[ModelBinder(BinderType = typeof(DecimalModelBinder))]
		[Range(AccountInitialBalanceMinValue,
			   AccountInitialBalanceMaxValue,
			   ErrorMessage = InvalidNumericLength)]
		public decimal Balance { get; set; }

		[Required(ErrorMessage = RequiredProperty)]
		public Guid? OwnerId { get; set; }

		[Required(ErrorMessage = RequiredProperty)]
		[Display(Name = "Account Type")]
		public Guid? AccountTypeId { get; set; }

		[Required(ErrorMessage = RequiredProperty)]
		[Display(Name = "Currency")]
		public Guid? CurrencyId { get; set; }
	}
}
