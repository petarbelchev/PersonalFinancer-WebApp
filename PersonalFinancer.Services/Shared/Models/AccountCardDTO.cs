#nullable disable

namespace PersonalFinancer.Services.Shared.Models
{
	public class AccountCardDTO
	{
        public Guid Id { get; set; }
	
		public Guid OwnerId { get; set; }

		public string Name { get; set; }

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; }
	}
}
