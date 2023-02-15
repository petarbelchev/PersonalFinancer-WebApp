namespace PersonalFinancer.Services.Account.Models
{
    public class TransactionShortViewModel
    {
        public Guid Id { get; set; }

        public string Account { get; init; } = null!;

        public string Currency { get; init; } = null!;

        public decimal Amount { get; init; }

        public string TransactionType { get; init; } = null!;

        public DateTime CreatedOn { get; init; }
    }
}
