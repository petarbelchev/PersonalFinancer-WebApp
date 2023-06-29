namespace PersonalFinancer.Services.Cache
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Shared.Contracts;
	using PersonalFinancer.Services.Shared.Models;
	using static PersonalFinancer.Common.Constants.AccountConstants;
	using static PersonalFinancer.Common.Constants.AccountTypeConstants;
	using static PersonalFinancer.Common.Constants.CategoryConstants;
	using static PersonalFinancer.Common.Constants.CurrencyConstants;

	public class MemoryCacheService<T> : ICacheService<T> where T : BaseApiEntity, new()
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

		public async Task<IEnumerable<TModel>> GetValues<TModel>(Guid userId, bool withDeleted)
			where TModel : BaseCacheableServiceModel, new()
		{
			var values = new List<TModel>();

			string cacheKey = this.GetCacheKey<TModel>(deletedCacheKey: false);
			values.AddRange(await this.GetValue<TModel>(userId, cacheKey));

			if (withDeleted)
			{
				string deletedCacheKey = this.GetCacheKey<TModel>(deletedCacheKey: true);
				values.AddRange(await this.GetValue<TModel>(userId, deletedCacheKey));
			}

			return values;
		}

		public void RemoveValues(Guid userId, bool notDeleted, bool deleted)
		{
			if (notDeleted)
			{
				string cacheKey = this.GetCacheKey<T>(deletedCacheKey: false);
				this.memoryCache.Remove(cacheKey + userId);
			}

			if (deleted)
			{
				string deletedCacheKey = this.GetCacheKey<T>(deletedCacheKey: true);
				this.memoryCache.Remove(deletedCacheKey + userId);
			}
		}

		private string GetCacheKey<TModel>(bool deletedCacheKey)
		{
			Type type = typeof(T);

			if (type == typeof(AccountDropdownDTO) || type == typeof(Account))
			{
				return deletedCacheKey
					? DeletedAccountCacheKeyValue
					: AccountCacheKeyValue;
			}
			else if (type == typeof(CurrencyDropdownDTO) || type == typeof(Currency))
			{
				return deletedCacheKey
					? DeletedCurrencyCacheKeyValue
					: CurrencyCacheKeyValue;
			}
			else if (type == typeof(CategoryDropdownDTO) || type == typeof(Category))
			{
				return deletedCacheKey
					? DeletedCategoryCacheKeyValue
					: CategoryCacheKeyValue;
			}
			else if (type == typeof(AccountTypeDropdownDTO) || type == typeof(AccountType))
			{
				return deletedCacheKey
					? DeletedAccTypeCacheKeyValue
					: AccTypeCacheKeyValue;
			}
			else
			{
				throw new InvalidOperationException($"{nameof(this.GetCacheKey)} do not support {nameof(TModel)}.");
			}
		}

		private async Task<IEnumerable<TModel>> GetValue<TModel>(Guid userId, string cacheKey)
			where TModel : BaseCacheableServiceModel, new()
		{
			bool isDeletedValue = cacheKey.StartsWith("deleted");
			string userCacheKey = cacheKey + userId;

			if (!this.memoryCache.TryGetValue(userCacheKey, out IEnumerable<TModel> value))
			{
				IQueryable<BaseApiEntity> query = this.QueryProvider(userId, isDeletedValue);

				value = await query
					.OrderBy(x => x.Name)
					.Select(x => this.mapper.Map<TModel>(x))
					.ToArrayAsync();

				this.memoryCache.Set(userCacheKey, value, TimeSpan.FromDays(3));
			}

			return value;
		}

		private IQueryable<BaseApiEntity> QueryProvider(Guid userId, bool isDeletedValue)
		{
			if (typeof(T) == typeof(Category))
			{
				return (this.repo as EfRepository<Category>)!.All()
					.Where(x => (x.OwnerId == userId 
									&& (isDeletedValue 
										? x.IsDeleted && x.Transactions.Any() 
										: !x.IsDeleted))
								|| (!isDeletedValue && x.Id == Guid.Parse(InitialBalanceCategoryId)));
			}
			else if (typeof(T) == typeof(AccountType))
			{
				return (this.repo as EfRepository<AccountType>)!.All()
					.Where(x => x.OwnerId == userId 
								&& (isDeletedValue 
									? x.IsDeleted && x.Accounts.Any(y => y.Transactions.Any() || !y.IsDeleted) 
									: !x.IsDeleted));
			}
			else if (typeof(T) == typeof(Currency))
			{
				return (this.repo as EfRepository<Currency>)!.All()
					.Where(x => x.OwnerId == userId 
								&& (isDeletedValue 
									? x.IsDeleted && x.Accounts.Any(y => y.Transactions.Any() || !y.IsDeleted) 
									: !x.IsDeleted));
			}
			else if (typeof(T) == typeof(Account))
			{
				return (this.repo as EfRepository<Account>)!.All()
					.Where(x => x.OwnerId == userId 
								&& (isDeletedValue 
									? x.IsDeleted && x.Transactions.Any() 
									: !x.IsDeleted));
			}
			else
			{
				throw new InvalidOperationException($"{nameof(this.QueryProvider)} do not support {nameof(T)}.");
			}
		}
	}
}
