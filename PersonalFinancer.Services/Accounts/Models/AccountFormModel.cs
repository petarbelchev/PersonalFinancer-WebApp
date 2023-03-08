using System.ComponentModel.DataAnnotations;

using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.Currencies.Models;
using static PersonalFinancer.Data.Constants.AccountConstants;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountFormModel
	{
		[Required(ErrorMessage = "Account name is required.")]
		[StringLength(AccountNameMaxLength, MinimumLength = AccountNameMinLength,
			ErrorMessage = "Account name must be between {2} and {1} characters long.")]
		public string Name { get; set; } = null!;

		[Required(ErrorMessage = "You need to set some balance.")]
		[DataType(DataType.Currency, ErrorMessage = "Balance must be a number.")]
		[Range(AccountInitialBalanceMinValue, AccountInitialBalanceMaxValue,
			ErrorMessage = "Ballace must be between {1} and {2}")]
		public decimal Balance { get; set; }

		[Display(Name = "Account Type")]
		public Guid AccountTypeId { get; set; }

		public IEnumerable<AccountTypeViewModel> AccountTypes { get; set; }
			= new List<AccountTypeViewModel>();

		[Display(Name = "Currency")]
		public Guid CurrencyId { get; set; }

		public IEnumerable<CurrencyViewModel> Currencies { get; set; }
			= new List<CurrencyViewModel>();
	}
}
