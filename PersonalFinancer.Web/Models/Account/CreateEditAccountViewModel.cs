namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Shared.Models;
	using System.ComponentModel.DataAnnotations;

	public class CreateEditAccountViewModel
	{
		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public Guid OwnerId { get; set; }

		[Display(Name = "Account Type")]
		public Guid AccountTypeId { get; set; }

		[Display(Name = "Currency")]
		public Guid CurrencyId { get; set; }

		public IEnumerable<DropdownDTO> OwnerAccountTypes { get; set; }
			= new List<DropdownDTO>();

		public IEnumerable<DropdownDTO> OwnerCurrencies { get; set; }
			= new List<DropdownDTO>();
	}
}
