namespace PersonalFinancer.Services.MemoryCacheService
{
	public interface IMemoryCacheService<T>
	{
		Task<IEnumerable<TResult>> GetValues<TResult>(string keyValue, Guid userId);
	}
}
