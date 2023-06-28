namespace PersonalFinancer.Services.Shared.Models
{
    public class TransactionTableDTO
    {
        public Guid Id { get; set; }

        public string AccountCurrencyName { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CategoryName { get; set; } = null!;

        public string TransactionType { get; set; } = null!;

        public string Reference { get; set; } = null!;
    }
}
