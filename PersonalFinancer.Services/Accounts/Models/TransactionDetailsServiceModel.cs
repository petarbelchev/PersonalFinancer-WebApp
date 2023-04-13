namespace PersonalFinancer.Services.Accounts.Models
{
    public class TransactionDetailsServiceModel
    {
        public string Id { get; set; } = null!;

        public string OwnerId { get; set; } = null!;

        public string AccountName { get; init; } = null!;

        public string AccountCurrencyName { get; init; } = null!;

        public decimal Amount { get; init; }

        public DateTime CreatedOn { get; init; }

        public string CategoryName { get; init; } = null!;

        public string TransactionType { get; init; } = null!;

        public string Refference { get; init; } = null!;
    }
}
