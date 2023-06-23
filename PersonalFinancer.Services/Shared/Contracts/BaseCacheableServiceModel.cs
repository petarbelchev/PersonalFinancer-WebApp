namespace PersonalFinancer.Services.Shared.Contracts
{
	public abstract class BaseCacheableServiceModel
	{
        public abstract Guid Id { get; set; }

        public abstract string Name { get; set; }
    }
}
