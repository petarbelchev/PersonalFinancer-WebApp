using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

using PersonalFinancer.Data.Enums;
using static PersonalFinancer.Data.Constants.TransactionConstants;

using PersonalFinancer.Web.Models.Shared;
using PersonalFinancer.Web.ModelBinders;

namespace PersonalFinancer.Web.Models.Accounts
{
	public class TransactionFormModel
	{
		[ModelBinder(BinderType = typeof(DecimalModelBinder))]
		[Range(TransactionMinValue, TransactionMaxValue,
			ErrorMessage = "Amount must be a number between {1} and {2}.")]
		public decimal Amount { get; set; }

		public string OwnerId { get; set; } = null!;

		[Required(ErrorMessage = "Category is required.")]
		[Display(Name = "Category")]
		public string CategoryId { get; set; } = null!;

		public IEnumerable<CategoryViewModel> UserCategories { get; set; }
			= new List<CategoryViewModel>();

		[Display(Name = "Account")]
		public string AccountId { get; set; } = null!;

		public IEnumerable<AccountDropdownViewModel> UserAccounts { get; set; }
			= new List<AccountDropdownViewModel>();

		[Required(ErrorMessage = "Date is required.")]
		[Display(Name = "Date")]
		[DataType(DataType.DateTime, ErrorMessage = "Please enter a valid Date.")]
		public DateTime CreatedOn { get; set; }

		[Required(ErrorMessage = "Payment Refference is required.")]
		[StringLength(TransactionRefferenceMaxLength,
			MinimumLength = TransactionRefferenceMinLength,
			ErrorMessage = "Payment Refference must be between {2} and {1} characters long.")]
		[Display(Name = "Payment Refference")]
		public string Refference { get; set; } = null!;

		[Display(Name = "Transaction Type")]
		public TransactionType TransactionType { get; set; }

		public TransactionType[] TransactionTypes { get; set; } = new TransactionType[]
		{
			TransactionType.Income,
			TransactionType.Expense
		};

		//public bool IsInitialBalance { get; set; }
	}
}
