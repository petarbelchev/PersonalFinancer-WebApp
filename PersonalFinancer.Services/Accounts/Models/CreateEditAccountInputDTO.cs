#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	public class CreateEditAccountInputDTO
	{
		public string Name { get; set; }

		public decimal Balance { get; set; }

		public Guid OwnerId { get; set; }

		public Guid AccountTypeId { get; set; }

		public Guid CurrencyId { get; set; }
	}
}
