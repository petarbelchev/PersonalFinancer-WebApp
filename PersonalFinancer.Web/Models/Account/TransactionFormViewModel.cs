namespace PersonalFinancer.Web.Models.Account
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models.Enums;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.ModelBinders;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.TransactionConstants;

    public class TransactionFormViewModel
	{
		[Required]
		[ModelBinder(BinderType = typeof(DecimalModelBinder))]
		[Range(TransactionMinValue, TransactionMaxValue,
			ErrorMessage = ValidationMessages.InvalidNumericLength)]
		public decimal Amount { get; set; }

		[Required]
		public Guid? OwnerId { get; set; }

		[Required]
		[Display(Name = "Category")]
		public Guid? CategoryId { get; set; }

		[Required]
		[Display(Name = "Account")]
		public Guid? AccountId { get; set; }

		[Required]
		[Display(Name = "Date")]
		public DateTime CreatedOn { get; set; }

		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(TransactionReferenceMaxLength,
			MinimumLength = TransactionReferenceMinLength,
			ErrorMessage = ValidationMessages.InvalidLength)]
		[Display(Name = "Payment Reference")]
		public string Reference { get; set; } = null!;

		[Required]
		[Display(Name = "Transaction Type")]
		public TransactionType TransactionType { get; set; }

		public IEnumerable<CategoryDropdownDTO> OwnerCategories { get; set; }
			= new List<CategoryDropdownDTO>();

		public IEnumerable<AccountDropdownDTO> OwnerAccounts { get; set; }
			= new List<AccountDropdownDTO>();

		public TransactionType[] TransactionTypes { get; set; } = new TransactionType[]
		{
			TransactionType.Income,
			TransactionType.Expense
		};
	}
}
