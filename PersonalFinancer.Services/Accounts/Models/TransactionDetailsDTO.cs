#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
    public class TransactionDetailsDTO
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string AccountName { get; init; }

        public string AccountCurrencyName { get; init; }

        public decimal Amount { get; init; }

        public DateTime CreatedOn { get; set; }

        public string CategoryName { get; init; }

        public string TransactionType { get; init; }

        public string Reference { get; init; }

        public bool IsInitialBalance { get; set; }
    }
}
