using PersonalFinancer.Services.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Services.Transactions.Models
{
	public class UserTransactionsExtendedViewModel
	{
        public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Controller = "Transaction",
			Action = "All"
		};

        public IEnumerable<TransactionExtendedViewModel> Transactions { get; set; }
			= new List<TransactionExtendedViewModel>();

		public string ElementsName { get; set; } = null!;

        public int ElementsPerPage { get; set; } = 10;

		public int Page { get; set; } = 1;

		public int TotalElements { get; set; }

		public int FirstElement 
			=> ElementsPerPage * (Page - 1) + 1;

		public int LastElement
		{
			get
			{
				int result = ElementsPerPage * Page;

				if (result > TotalElements)
				{
					result = TotalElements;
				}

				return result;
			}
		}

		public int Pages
		{
			get
			{
				int result = TotalElements / ElementsPerPage;

				if (TotalElements % ElementsPerPage != 0)
				{
					result++;
				}

				return result;
			}
		}

		
		[Required(ErrorMessage = "Start Date is required.")]
		[DataType(DataType.DateTime)]
		[Display(Name = "Start Date")]
		public DateTime? StartDate { get; set; }

		[Required(ErrorMessage = "End Date is required.")]
		[DataType(DataType.DateTime)]
		[Display(Name = "End Date")]
		public DateTime? EndDate { get; set; }

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
