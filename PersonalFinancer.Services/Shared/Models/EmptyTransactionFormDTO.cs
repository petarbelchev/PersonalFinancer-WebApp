using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Categories.Models;

namespace PersonalFinancer.Services.Shared.Models
{
	public class EmptyTransactionFormDTO
	{
        public string OwnerId { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public IEnumerable<AccountDTO> UserAccounts { get; set; } = null!;

        public IEnumerable<CategoryOutputDTO> UserCategories { get; set; } = null!;
    }
}
