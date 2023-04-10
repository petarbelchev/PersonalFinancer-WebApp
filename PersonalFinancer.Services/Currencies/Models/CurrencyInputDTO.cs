namespace PersonalFinancer.Services.Currencies.Models
{
	public class CurrencyInputDTO
    {
        public string Name { get; init; } = null!;

        public string OwnerId { get; set; } = null!;
    }
}
