namespace PersonalFinancer.Services.Shared.Models
{
	using PersonalFinancer.Services.Shared.Contracts;

	public class CurrencyServiceModel : BaseCacheableServiceModel
    {
        public override Guid Id { get; set; }

        public override string Name { get; set; } = null!;
    }
}
