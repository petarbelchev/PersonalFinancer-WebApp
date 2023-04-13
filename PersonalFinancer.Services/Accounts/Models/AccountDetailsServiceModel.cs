namespace PersonalFinancer.Services.Accounts.Models
{
	using Services.Shared.Models;

	public class AccountDetailsServiceModel : AccountDetailsShortServiceModel
	{
		public string Id { get; set; } = null!;

        public string OwnerId { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<TransactionTableServiceModel> Transactions { get; set; } = null!;

        public int TotalAccountTransactions { get; set; }
    }
}
