using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace PersonalFinancer.Tests.Mocks
{
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
