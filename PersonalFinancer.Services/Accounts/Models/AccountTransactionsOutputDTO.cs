namespace PersonalFinancer.Services.Accounts.Models
{
	using Services.Shared.Models;

	public class AccountTransactionsOutputDTO
	{
		public IEnumerable<TransactionTableDTO> Transactions { get; set; } = null!;

		public int Page { get; set; }

		public int AllTransactionsCount { get; set; }

		public string TransactionDetailsUrl { get; set; } = null!;
	}
}
