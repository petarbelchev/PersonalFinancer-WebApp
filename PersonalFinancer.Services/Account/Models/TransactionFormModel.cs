﻿namespace PersonalFinancer.Services.Account.Models
{
	using System.ComponentModel.DataAnnotations;

	using Category.Models;
	using Data.Enums;
	using static Data.DataConstants.Transaction;
	
	public class TransactionFormModel
	{
		[DataType(DataType.Currency)]
		[Range(TransactionMinValue, TransactionMaxValue,
			ErrorMessage = "Amount must be between {1} and {2}.")]
		public decimal Amount { get; set; }

		[Display(Name = "Category")]
		public Guid CategoryId { get; set; }

		public List<CategoryViewModel> Categories { get; set; }
			= new List<CategoryViewModel>();

		[Display(Name = "Account")]
		public Guid AccountId { get; set; }

		public List<AccountDropdownViewModel> Accounts { get; set; }
			= new List<AccountDropdownViewModel>();

		[Display(Name = "Date")]
		public DateTime CreatedOn { get; set; }

		[Required]
		[StringLength(TransactionRefferenceMaxLength,
			MinimumLength = TransactionRefferenceMinLength,
			ErrorMessage = "Payment Refference must be between {2} and {1} characters long.")]
		[Display(Name = "Payment Refference")]
		public string Refference { get; set; } = null!;

		[Display(Name = "Transaction Type")]
		public TransactionType TransactionType { get; set; }

		public List<TransactionType> TransactionTypes { get; set; }
			= new List<TransactionType>();
	}
}
