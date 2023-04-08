namespace PersonalFinancer.Services.Accounts.Models
{
	using Services.Shared.Models;
	
	public class AccountDetailsOutputDTO : AccountCardDTO
	{
		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public string OwnerId { get; set; } = null!;

		public IEnumerable<TransactionTableDTO> Transactions { get; set; } = null!;

		public int AllTransactionsCount { get; set; }
	}
}
