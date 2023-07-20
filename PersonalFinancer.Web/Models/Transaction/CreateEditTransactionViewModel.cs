namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditTransactionViewModel : CreateEditTransactionInputModel
    {
        public IEnumerable<DropdownDTO> OwnerCategories { get; set; }
            = new List<DropdownDTO>();

        public IEnumerable<DropdownDTO> OwnerAccounts { get; set; }
            = new List<DropdownDTO>();
    }
}
