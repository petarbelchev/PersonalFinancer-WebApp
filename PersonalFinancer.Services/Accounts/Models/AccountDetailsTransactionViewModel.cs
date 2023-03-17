namespace PersonalFinancer.Services.Accounts.Models
{
    public class AccountDetailsTransactionViewModel
    {
        public string Id { get; set; } = null!;

        public decimal Amount { get; init; }

        public string CurrencyName { get; set; } = null!;

        public string CategoryName { get; init; } = null!;

        public string TransactionType { get; init; } = null!;

        public DateTime CreatedOn { get; init; }

        public string Refference { get; init; } = null!;
    }
}
