using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class UserTransactionsApiOutputDTO
	{
        public IEnumerable<TransactionTableDTO> Transactions { get; set; } = null!;

        public int Page { get; set; }

        public int AllTransactionsCount { get; set; }
    }
}
