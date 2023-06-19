namespace PersonalFinancer.Services.MemoryCacheService
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Data.Repositories;

	public class MemoryCacheService<T> : IMemoryCacheService<T> where T : CacheableEntity
	{
		private readonly IMemoryCache memoryCache;
		private readonly IEfRepository<T> repo;
		private readonly IMapper mapper;

		public MemoryCacheService(
			IMemoryCache memoryCache,
			IEfRepository<T> repo,
			IMapper mapper)
		{
			this.memoryCache = memoryCache;
			this.repo = repo;
			this.mapper = mapper;
		}

		public async Task<IEnumerable<TResult>> GetValues<TResult>(string keyValue, Guid userId)
		{
			if (!this.memoryCache.TryGetValue(keyValue + userId, out TResult[] value))
			{
				value = await this.repo.All()
					.Where(c => c.OwnerId == userId && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => this.mapper.Map<TResult>(c))
					.ToArrayAsync();

				this.memoryCache.Set(keyValue + userId, value, TimeSpan.FromDays(3));
			}

			return value;
		}
	}
}
