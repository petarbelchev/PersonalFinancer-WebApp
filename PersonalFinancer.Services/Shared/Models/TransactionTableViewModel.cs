namespace PersonalFinancer.Services.Shared.Models
{
    public class TransactionTableViewModel
    {
        public string Id { get; set; } = null!;

        public string AccountCurrencyName { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CategoryName { get; set; } = null!;

        public string TransactionType { get; set; } = null!;

        public string Refference { get; set; } = null!;
    }
}
