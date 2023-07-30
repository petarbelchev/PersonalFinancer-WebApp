namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Shared.Models;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Messages.ValidationMessages;

	public class CreateEditAccountViewModel
	{
		[Required(ErrorMessage = RequiredProperty)]
		public string Name { get; set; } = null!;

		[Required(ErrorMessage = RequiredProperty)]
		public decimal Balance { get; set; }

		public Guid OwnerId { get; set; }

		[Required(ErrorMessage = RequiredProperty)]
		[Display(Name = "Account Type")]
		public Guid AccountTypeId { get; set; }

		[Required(ErrorMessage = RequiredProperty)]
		[Display(Name = "Currency")]
		public Guid CurrencyId { get; set; }

		public IEnumerable<DropdownDTO> OwnerAccountTypes { get; set; }
			= new List<DropdownDTO>();

		public IEnumerable<DropdownDTO> OwnerCurrencies { get; set; }
			= new List<DropdownDTO>();
	}
}
