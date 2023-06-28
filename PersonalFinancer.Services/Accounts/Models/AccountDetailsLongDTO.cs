#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
    using PersonalFinancer.Services.Shared.Models;

    public class AccountDetailsLongDTO
    {
        public Guid Id { get; set; }
		
        public string Name { get; set; }
		
        public decimal Balance { get; set; }
		
        public string AccountTypeName { get; set; }
		
        public string CurrencyName { get; set; }

        public Guid OwnerId { get; set; }

		public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<TransactionTableDTO> Transactions { get; set; }

        public int TotalAccountTransactions { get; set; }
    }
}
