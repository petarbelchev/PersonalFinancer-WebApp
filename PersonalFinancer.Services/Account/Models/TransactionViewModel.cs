namespace PersonalFinancer.Services.Account.Models
{
    public class TransactionViewModel
    {
        public int Id { get; set; }

        public string Account { get; init; } = null!;

        public string Currency { get; init; } = null!;

        public decimal Amount { get; init; }

        public string Category { get; init; } = null!;

        public string TransactionType { get; init; } = null!;

        public DateTime CreatedOn { get; init; }

        public string Refference { get; init; } = null!;
    }
}
