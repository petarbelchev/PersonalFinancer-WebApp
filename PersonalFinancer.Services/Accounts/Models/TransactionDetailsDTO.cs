#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class TransactionDetailsDTO : TransactionTableDTO, IHaveOwner
    {
        public Guid OwnerId { get; set; }

        public string AccountName { get; init; }

        public bool IsInitialBalance { get; set; }
    }
}
