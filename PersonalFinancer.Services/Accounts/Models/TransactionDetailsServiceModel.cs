namespace PersonalFinancer.Services.Accounts.Models
{
    public class TransactionDetailsServiceModel
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string AccountName { get; init; } = null!;

        public string AccountCurrencyName { get; init; } = null!;

        public decimal Amount { get; init; }

        public DateTime CreatedOn { get; set; }

        public string CategoryName { get; init; } = null!;

        public string TransactionType { get; init; } = null!;

        public string Reference { get; init; } = null!;

        public bool IsInitialBalance { get; set; }
    }
}
