using PersonalFinancer.Services.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Services.Accounts.Models
{
    public class UserTransactionsViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public IEnumerable<TransactionTableViewModel> Transactions { get; set; }
            = new List<TransactionTableViewModel>();

        public RoutingModel Routing { get; set; } = new RoutingModel
        {
            Controller = "Transactions",
            Action = "All"
        };

        public PaginationModel Pagination { get; set; }
            = new PaginationModel();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new ValidationResult(
                    "Start Date must be before End Date.",
                    new[] { "StartDate" });

                yield return new ValidationResult(
                    "End Date must be after Start Date.",
                    new[] { "EndDate" });
            }
        }
    }
}
