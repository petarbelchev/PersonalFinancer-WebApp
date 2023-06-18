namespace PersonalFinancer.Services.Accounts.Models
{
    using PersonalFinancer.Services.Shared.Models;

    public class TransactionsServiceModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<TransactionTableServiceModel> Transactions { get; set; } = null!;

        public int TotalTransactionsCount { get; set; }
    }
}
