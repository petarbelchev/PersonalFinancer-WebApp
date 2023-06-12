namespace PersonalFinancer.Services.Shared.Models
{	
	public class AccountCardServiceModel
	{
        public Guid Id { get; set; }
	
		public Guid OwnerId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

        public string CurrencyName { get; set; } = null!;
	}
}
