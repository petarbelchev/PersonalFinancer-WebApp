using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.User.Models
{
	public class UserAccountsAndCategoriesServiceModel
	{
		public string OwnerId { get; set; } = null!;

		public IEnumerable<AccountServiceModel> UserAccounts { get; set; } = null!;

		public IEnumerable<CategoryServiceModel> UserCategories { get; set; } = null!;
	}
}
