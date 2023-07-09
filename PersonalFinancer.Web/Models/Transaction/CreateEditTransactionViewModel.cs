namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditTransactionViewModel : CreateEditTransactionInputModel
    {
        public IEnumerable<CategoryDropdownDTO> OwnerCategories { get; set; }
            = new List<CategoryDropdownDTO>();

        public IEnumerable<AccountDropdownDTO> OwnerAccounts { get; set; }
            = new List<AccountDropdownDTO>();
    }
}
