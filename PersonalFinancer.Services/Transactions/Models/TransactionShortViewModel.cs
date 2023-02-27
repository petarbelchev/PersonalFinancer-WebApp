namespace PersonalFinancer.Services.Transactions.Models
{
    public class TransactionShortViewModel
    {
        public Guid Id { get; set; }

        public string AccountName { get; init; } = null!;

        public string AccountCurrencyName { get; init; } = null!;

        public decimal Amount { get; init; }

        public string TransactionType { get; init; } = null!;

        public DateTime CreatedOn { get; init; }
    }
}
