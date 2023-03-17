using System.ComponentModel.DataAnnotations;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Categories.Models;
using static PersonalFinancer.Data.Constants.TransactionConstants;

namespace PersonalFinancer.Services.Transactions.Models
{
	public class TransactionFormModel
	{
		[Required(ErrorMessage = "Amount is required.")]
		[DataType(DataType.Currency, ErrorMessage = "Amount must be a number.")]
		[Range(TransactionMinValue, TransactionMaxValue, 
			ErrorMessage = "Amount must be between {1} and {2}.")]
		public decimal Amount { get; set; }
		
		[Required(ErrorMessage = "Category is required.")]
		[Display(Name = "Category")]
		public string CategoryId { get; set; } = null!;

		public List<CategoryViewModel> Categories { get; set; }
			= new List<CategoryViewModel>();

		[Display(Name = "Account")]
		public string AccountId { get; set; } = null!;

		public List<AccountDropdownViewModel> Accounts { get; set; }
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

		public List<TransactionType> TransactionTypes { get; set; }
			= new List<TransactionType>();
	}
}
