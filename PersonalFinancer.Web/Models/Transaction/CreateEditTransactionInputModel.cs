namespace PersonalFinancer.Web.Models.Transaction
{
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Web.CustomModelBinders;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.TransactionConstants;
	using static PersonalFinancer.Common.Messages.ValidationMessages;

	public class CreateEditTransactionInputModel
	{
		public CreateEditTransactionInputModel()
			=> this.CreatedOnLocalTime = DateTime.Now;

		[Required(ErrorMessage = RequiredProperty)]
		[ModelBinder(BinderType = typeof(DecimalModelBinder))]
		[Range(TransactionMinValue, TransactionMaxValue,
			ErrorMessage = InvalidNumericLength)]
		public decimal Amount { get; set; }

		[Required]
		public Guid? OwnerId { get; set; }

		[Required(ErrorMessage = RequiredProperty)]
		[Display(Name = "Category")]
		public Guid? CategoryId { get; set; }

		[Required(ErrorMessage = RequiredProperty)]
		[Display(Name = "Account")]
		public Guid? AccountId { get; set; }

		[Required(ErrorMessage = RequiredProperty)]
		[Display(Name = "Date")]
		[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
		public DateTime CreatedOnLocalTime { get; set; }

		[Required(ErrorMessage = RequiredProperty)]
		[StringLength(TransactionReferenceMaxLength,
			MinimumLength = TransactionReferenceMinLength,
			ErrorMessage = InvalidLength)]
		[Display(Name = "Payment Reference")]
		public string Reference { get; set; } = null!;

		[Required(ErrorMessage = RequiredProperty)]
		[Display(Name = "Transaction Type")]
		public TransactionType TransactionType { get; set; }
	}
}
