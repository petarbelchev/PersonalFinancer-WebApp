namespace PersonalFinancer.Services.User.Models
{
    using PersonalFinancer.Services.Shared.Models;

    public class UserAccountsAndCategoriesServiceModel
    {
        public Guid OwnerId { get; set; }

        public IEnumerable<AccountServiceModel> UserAccounts { get; set; } = null!;

        public IEnumerable<CategoryServiceModel> UserCategories { get; set; } = null!;
    }
}
