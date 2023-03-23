using System.ComponentModel.DataAnnotations;
using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.User.Models
{
    public class UserDashboardViewModel : IValidatableObject
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

        public IEnumerable<AccountCardViewModel> Accounts { get; set; }
            = new List<AccountCardViewModel>();

        public Dictionary<string, CashFlowViewModel> CurrenciesCashFlow { get; set; }
            = new Dictionary<string, CashFlowViewModel>();

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
