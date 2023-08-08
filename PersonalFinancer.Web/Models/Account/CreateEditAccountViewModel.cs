namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditAccountViewModel : CreateEditAccountInputModel
	{
		public IEnumerable<DropdownDTO> OwnerAccountTypes { get; set; }
			= new List<DropdownDTO>();

		public IEnumerable<DropdownDTO> OwnerCurrencies { get; set; }
			= new List<DropdownDTO>();
	}
}
