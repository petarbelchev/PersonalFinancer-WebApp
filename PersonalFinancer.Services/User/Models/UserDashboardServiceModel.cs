namespace PersonalFinancer.Services.User.Models
{
    using Services.Shared.Models;

    public class UserDashboardServiceModel
	{
		public IEnumerable<TransactionTableServiceModel> LastTransactions { get; set; } = null!;

		public IEnumerable<AccountCardServiceModel> Accounts { get; set; } = null!;

		public IEnumerable<CurrencyCashFlowServiceModel> CurrenciesCashFlow { get; set; } = null!;
	}
}
