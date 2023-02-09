using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Category.Models;
using System.ComponentModel.DataAnnotations;
using static PersonalFinancer.Data.DataConstants.Transaction;

namespace PersonalFinancer.Services.Account.Models
{
    public class CreateTransactionFormModel
    {
        [DataType(DataType.Currency)]
        [Range(TransactionMinValue, TransactionMaxValue,
            ErrorMessage = "Amount must be between {1} and {2}.")]
        public decimal Amount { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
            = new List<CategoryViewModel>();

        [Display(Name = "Account")]
        public int AccountId { get; set; }
        public IEnumerable<AccountViewModel> Accounts { get; set; }
            = new List<AccountViewModel>();

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(TransactionRefferenceMaxLength,
            MinimumLength = TransactionRefferenceMinLength,
            ErrorMessage = "Payment Refference must be between {2} and {1} characters long.")]
        [Display(Name = "Payment Refference")]
        public string PaymentRefference { get; set; } = null!;

        [Display(Name = "Transaction Type")]
        public TransactionType TransactionType { get; set; }
    }
}
