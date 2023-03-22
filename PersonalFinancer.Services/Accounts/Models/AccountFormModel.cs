using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.Currencies.Models;
using PersonalFinancer.Services.Infrastructure;
using static PersonalFinancer.Data.Constants.AccountConstants;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountFormModel
	{
		[Required(ErrorMessage = "Account name is required.")]
		[StringLength(AccountNameMaxLength, MinimumLength = AccountNameMinLength,
			ErrorMessage = "Account name must be between {2} and {1} characters long.")]
		public string Name { get; set; } = null!;

		[ModelBinder(BinderType = typeof(DecimalModelBinder))]
		[Range(AccountInitialBalanceMinValue, AccountInitialBalanceMaxValue,
			ErrorMessage = "Ballace must be a number between {1} and {2}")]
		public decimal? Balance { get; set; }

        public string OwnerId { get; set; } = null!;

        [Required(ErrorMessage = "Account Type name is required.")]
		[Display(Name = "Account Type")]
		public string AccountTypeId { get; set; } = null!;

		public IEnumerable<AccountTypeViewModel> AccountTypes { get; set; }
			= new List<AccountTypeViewModel>();
		
		[Required(ErrorMessage = "Currency name is required.")]
		[Display(Name = "Currency")]
		public string CurrencyId { get; set; }  = null!;

		public IEnumerable<CurrencyViewModel> Currencies { get; set; }
			= new List<CurrencyViewModel>();
	}
}
