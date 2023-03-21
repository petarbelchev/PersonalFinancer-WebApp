using PersonalFinancer.Data.Enums;

namespace PersonalFinancer.Services.Transactions.Models
{
	public class TransactionServiceModel
	{
        public string CurrencyName { get; set; } = null!;

        public TransactionType TransactionType { get; set; }

        public decimal Amount { get; set; }
    }
}
