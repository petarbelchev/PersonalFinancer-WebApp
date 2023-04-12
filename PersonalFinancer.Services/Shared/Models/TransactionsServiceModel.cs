namespace PersonalFinancer.Services.Shared.Models
{
    public class TransactionsServiceModel
    {
        public IEnumerable<TransactionTableServiceModel> Transactions { get; set; } = null!;

        public int TotalTransactionsCount { get; set; }
    }
}
