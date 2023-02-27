namespace PersonalFinancer.Services.Transactions.Models
{
    using System.ComponentModel.DataAnnotations;

    using Accounts.Models;
    using Category.Models;
    using Data.Enums;
    using static Data.DataConstants.TransactionConstants;

    public class TransactionFormModel
    {
        [Required(ErrorMessage = "Amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Amount must be a number.")]
        [Range(TransactionMinValue, TransactionMaxValue, ErrorMessage = "Amount must be between {1} and {2}.")]
        public decimal Amount { get; set; }

        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        public List<CategoryViewModel> Categories { get; set; }
            = new List<CategoryViewModel>();

        [Display(Name = "Account")]
        public Guid AccountId { get; set; }

        public List<AccountDropdownViewModel> Accounts { get; set; }
            = new List<AccountDropdownViewModel>();

        [Required(ErrorMessage = "Date is required.")]
        [Display(Name = "Date")]
        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid Date.")]
        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "Payment Refference is required.")]
        [StringLength(TransactionRefferenceMaxLength, MinimumLength = TransactionRefferenceMinLength,
            ErrorMessage = "Payment Refference must be between {2} and {1} characters long.")]
        [Display(Name = "Payment Refference")]
        public string Refference { get; set; } = null!;

        [Display(Name = "Transaction Type")]
        public TransactionType TransactionType { get; set; }

        public List<TransactionType> TransactionTypes { get; set; }
            = new List<TransactionType>();
    }
}
