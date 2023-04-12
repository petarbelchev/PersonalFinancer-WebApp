namespace PersonalFinancer.Services.Shared.Models
{	
	public class AccountCardServiceModel
	{
        public string Id { get; set; } = null!;
	
		public string OwnerId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

        public string CurrencyName { get; set; } = null!;
	}
}
