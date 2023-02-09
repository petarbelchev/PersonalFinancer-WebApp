﻿using PersonalFinancer.Services.Currency.Models;
using System.ComponentModel.DataAnnotations;
using static PersonalFinancer.Data.DataConstants.Account;

namespace PersonalFinancer.Services.Account.Models
{
	public class CreateAccountFormModel
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
		public int AccountTypeId { get; set; }

		public IEnumerable<AccountTypeViewModel> AccountTypes { get; set; }
			= new List<AccountTypeViewModel>();

		[Display(Name = "Currency")]
		public int CurrencyId { get; set; }

		public IEnumerable<CurrencyViewModel> Currencies { get; set; }
			= new List<CurrencyViewModel>();
	}
}
