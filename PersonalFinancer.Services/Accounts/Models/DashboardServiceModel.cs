using System.ComponentModel.DataAnnotations;

using PersonalFinancer.Services.Transactions.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class DashboardServiceModel
	{
		[Required(ErrorMessage = "Start Date is required.")]
		[DataType(DataType.Date)]
		[Display(Name = "Start Date")]
		public DateTime? StartDate { get; set; }

		[Required(ErrorMessage = "End Date is required.")]
		[DataType(DataType.Date)]
		[Display(Name = "End Date")]
		public DateTime? EndDate { get; set; }

		public IEnumerable<TransactionShortViewModel> LastTransactions { get; set; }
			= new List<TransactionShortViewModel>();

		public IEnumerable<AccountCardViewModel> Accounts { get; set; }
			= new List<AccountCardViewModel>();

		public Dictionary<string, CashFlowViewModel> CurrenciesCashFlow { get; set; }
			= new Dictionary<string, CashFlowViewModel>();
	}
}
