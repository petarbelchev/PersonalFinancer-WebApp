namespace PersonalFinancer.Services.Accounts.Models
{
	using Services.Shared.Models;

	public class TransactionFormServiceModel : TransactionFormShortServiceModel
	{
        public IEnumerable<AccountServiceModel> UserAccounts { get; set; } = null!;

        public IEnumerable<CategoryServiceModel> UserCategories { get; set; } = null!;
    }
}
