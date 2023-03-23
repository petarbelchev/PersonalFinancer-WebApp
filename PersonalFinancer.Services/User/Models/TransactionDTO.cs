using PersonalFinancer.Data.Enums;

namespace PersonalFinancer.Services.User.Models
{
    public class TransactionDTO
    {
        public string CurrencyName { get; set; } = null!;

        public TransactionType TransactionType { get; set; }

        public decimal Amount { get; set; }
    }
}
