using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsServiceModel : AccountDetailsShortServiceModel
	{
		public string Id { get; set; } = null!;

		public string OwnerId { get; set; } = null!;

		public string AccountTypeName { get; set; } = null!;

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public IEnumerable<TransactionTableServiceModel> Transactions { get; set; } = null!;

		public int TotalAccountTransactions { get; set; }
	}
}
