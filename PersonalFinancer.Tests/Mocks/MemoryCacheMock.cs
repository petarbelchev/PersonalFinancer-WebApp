namespace PersonalFinancer.Tests.Mocks
{
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;

    public class MemoryCacheMock
	{
		public static MemoryCache Instance
		{
			get
			{
				IOptions<MemoryCacheOptions> options = new MemoryCacheOptions();
				var memoryCache = new MemoryCache(options.Value);
				return memoryCache;
			}
		}
	}
}
