namespace PersonalFinancer.Services.Accounts.Models
{
    using PersonalFinancer.Services.Shared.Models;

    public class AccountDetailsServiceModel : AccountDetailsShortServiceModel
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string AccountTypeName { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<TransactionTableServiceModel> Transactions { get; set; } = null!;

        public int TotalAccountTransactions { get; set; }
    }
}
