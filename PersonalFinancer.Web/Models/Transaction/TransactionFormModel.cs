namespace PersonalFinancer.Web.Models.Transaction
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Data.Models.Enums;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.ModelBinders;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.TransactionConstants;

    public class TransactionFormModel
    {
        [Required(ErrorMessage = "Please enter an Amount.")]
        [ModelBinder(BinderType = typeof(DecimalModelBinder))]
        [Range(TransactionMinValue, TransactionMaxValue,
            ErrorMessage = "Amount must be a number between {1} and {2}.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Owner Id is required.")]
        public Guid? OwnerId { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public Guid? CategoryId { get; set; }

        public IEnumerable<CategoryServiceModel> UserCategories { get; set; }
            = new List<CategoryServiceModel>();

        [Required(ErrorMessage = "Account is required.")]
        [Display(Name = "Account")]
        public Guid? AccountId { get; set; }

        public IEnumerable<AccountServiceModel> UserAccounts { get; set; }
            = new List<AccountServiceModel>();

        [Required(ErrorMessage = "Date is required.")]
        [Display(Name = "Date")]
        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid Date.")]
        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "Payment Reference is required.")]
        [StringLength(TransactionReferenceMaxLength,
            MinimumLength = TransactionReferenceMinLength,
            ErrorMessage = "Payment Reference must be between {2} and {1} characters long.")]
        [Display(Name = "Payment Reference")]
        public string Reference { get; set; } = null!;

        [Display(Name = "Transaction Type")]
        public TransactionType TransactionType { get; set; }

        public TransactionType[] TransactionTypes => this.IsInitialBalance ?
            new TransactionType[] { TransactionType.Income }
            : new TransactionType[]
            {
                TransactionType.Income,
                TransactionType.Expense
            };

        public bool IsInitialBalance { get; set; }
    }
}
