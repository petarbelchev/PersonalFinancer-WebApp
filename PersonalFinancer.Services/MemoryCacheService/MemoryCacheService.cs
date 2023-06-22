namespace PersonalFinancer.Services.MemoryCacheService
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Data.Repositories;
	using static PersonalFinancer.Data.Constants.CategoryConstants;

	public class MemoryCacheService<T> : IMemoryCacheService<T> where T : ApiEntity
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
				IQueryable<T> query = this.repo.All();

				if (typeof(T) == typeof(Category))
				{
					query = query.Where(x =>
						(x.OwnerId == userId && !x.IsDeleted)
						|| x.Id == Guid.Parse(InitialBalanceCategoryId));
				}
				else
				{
					query = query.Where(x => x.OwnerId == userId && !x.IsDeleted);
				}

				value = await query
					.OrderBy(c => c.Name)
					.Select(c => this.mapper.Map<TResult>(c))
					.ToArrayAsync();

				this.memoryCache.Set(keyValue + userId, value, TimeSpan.FromDays(3));
			}

			return value;
		}
	}
}
