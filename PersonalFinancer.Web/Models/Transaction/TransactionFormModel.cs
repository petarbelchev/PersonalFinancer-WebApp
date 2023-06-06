using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Data.Models.Enums;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure.ModelBinders;
using System.ComponentModel.DataAnnotations;
using static PersonalFinancer.Data.Constants.TransactionConstants;

namespace PersonalFinancer.Web.Models.Transaction
{
    public class TransactionFormModel
	{
		[Required(ErrorMessage = "Plaese enter an Amount.")]
		[ModelBinder(BinderType = typeof(DecimalModelBinder))]
		[Range(TransactionMinValue, TransactionMaxValue,
			ErrorMessage = "Amount must be a number between {1} and {2}.")]
		public decimal Amount { get; set; }

		[Required]
		public string OwnerId { get; set; } = null!;

		[Required(ErrorMessage = "Category is required.")]
		[Display(Name = "Category")]
		public string CategoryId { get; set; } = null!;

		public IEnumerable<CategoryServiceModel> UserCategories { get; set; }
			= new List<CategoryServiceModel>();

		[Display(Name = "Account")]
		public string AccountId { get; set; } = null!;

		public IEnumerable<AccountServiceModel> UserAccounts { get; set; }
			= new List<AccountServiceModel>();

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

		public TransactionType[] TransactionTypes => IsInitialBalance ?
			new TransactionType[] { TransactionType.Income }
			: new TransactionType[]
			{
				TransactionType.Income,
				TransactionType.Expense
			};

		public bool IsInitialBalance { get; set; }
	}
}
