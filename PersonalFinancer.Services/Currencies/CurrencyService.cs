using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Currencies.Models;
using static PersonalFinancer.Data.Constants.CurrencyConstants;

namespace PersonalFinancer.Services.Currencies
{
	public class CurrencyService : ICurrencyService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public CurrencyService(
			PersonalFinancerDbContext data,
			IMapper mapper,
			IMemoryCache memoryCache)
		{
			this.data = data;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
		}

		/// <summary>
		/// Throws ArgumentException if given name exists.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task<CurrencyViewModel> CreateCurrency(string userId, string currencyName)
		{
			Currency? currency = await data.Currencies
				.FirstOrDefaultAsync(c => c.Name == currencyName && c.OwnerId == userId);

			if (currency != null)
			{
				if (currency.IsDeleted == false)
					throw new ArgumentException("Currency with the same name exist!");

				currency.IsDeleted = false;
				currency.Name = currencyName.Trim();
			}
			else
			{
				currency = new Currency
				{
					Id = Guid.NewGuid().ToString(),
					Name = currencyName,
					OwnerId = userId
				};

				data.Currencies.Add(currency);
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);

			return mapper.Map<CurrencyViewModel>(currency);
		}

		/// <summary>
		/// Throws InvalidOperationException when Currency does not exist
		/// and ArgumentException when User is not owner.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteCurrency(string currencyId, string ownerId)
		{
			Currency? currency = await data.Currencies.FindAsync(currencyId);

			if (currency == null)
				throw new InvalidOperationException("Currency does not exist.");

			if (currency.OwnerId != ownerId)
				throw new ArgumentException("Can't delete someone else Currency.");

			currency.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + ownerId);
		}

		public async Task<IEnumerable<CurrencyViewModel>> GetUserCurrencies(string ownerId)
		{
			string cacheKey = CacheKeyValue + ownerId;

			var currencies = await memoryCache.GetOrCreateAsync<IEnumerable<CurrencyViewModel>>(cacheKey, async cacheEntry =>
			{
				cacheEntry.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.Currencies
					.Where(c => c.OwnerId == ownerId && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => mapper.Map<CurrencyViewModel>(c))
					.ToArrayAsync();
			});

			return currencies;
		}
	}
}
