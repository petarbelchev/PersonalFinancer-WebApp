using System.ComponentModel.DataAnnotations;

using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
    public class DetailsAccountViewModel : IValidatableObject
	{
		[Required(ErrorMessage = "Start Date is required.")]
		[Display(Name = "Start Date")]
		public DateTime? StartDate { get; set; }

		[Required(ErrorMessage = "End Date is required.")]
		[Display(Name = "End Date")]
		public DateTime? EndDate { get; set; }
		
		public string? Name { get; set; }

		public decimal Balance { get; set; }

		public string? CurrencyName { get; set; }

		public IEnumerable<TransactionTableViewModel> Transactions { get; set; }
			= new List<TransactionTableViewModel>();

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Controller = "Account",
			Action = "AccountDetails"
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
