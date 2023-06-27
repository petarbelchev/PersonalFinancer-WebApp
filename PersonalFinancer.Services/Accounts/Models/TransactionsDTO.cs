#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
    using PersonalFinancer.Services.Shared.Models;

    public class TransactionsDTO
    {
        public IEnumerable<TransactionTableDTO> Transactions { get; set; }

        public int TotalTransactionsCount { get; set; }
    }
}
