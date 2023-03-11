namespace PersonalFinancer.Services.Accounts.Models
{
    public class AccountDetailsTransactionViewModel
    {
        public Guid Id { get; set; }

        public decimal Amount { get; init; }

        public string CurrencyName { get; set; } = null!;

        public string CategoryName { get; init; } = null!;

        public string TransactionType { get; init; } = null!;

        public DateTime CreatedOn { get; init; }

        public string Refference { get; init; } = null!;
    }
}
