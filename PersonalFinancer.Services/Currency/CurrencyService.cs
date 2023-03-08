namespace PersonalFinancer.Services.Currency
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using System.Collections.Generic;

	using Data;
	using Data.Models;
	using static Data.Constants.CurrencyConstants;
	using Services.Currency.Models;

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
		/// Creates new Currency with given Name. If you try to create a new Currency with name that other Currency have, throws exception.
		/// </summary>
		/// <returns>View Model with Id, Name and User Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CurrencyViewModel> CreateCurrency(string userId, string currencyName)
		{
			Currency? currency = await data.Currencies
				.FirstOrDefaultAsync(c => 
					c.Name == currencyName 
					&& (c.UserId == userId || c.UserId == null));

			if (currency != null)
			{
				if (currency.IsDeleted == false)
				{
					throw new InvalidOperationException("Currency with the same name exist!");
				}

				currency.IsDeleted = false;
			}
			else
			{
				currency = new Currency
				{
					Name = currencyName,
					UserId = userId
				};

				data.Currencies.Add(currency);
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);

			return mapper.Map<CurrencyViewModel>(currency);
		}

		/// <summary>
		/// Delete Currency with given Id or throws exception when Currency does not exist or User is not owner.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteCurrency(Guid currencyId, string userId)
		{
			Currency? currency = await data.Currencies.FindAsync(currencyId);

			if (currency == null)
			{
				throw new ArgumentNullException("Currency does not exist.");
			}

			if (currency.UserId != userId)
			{
				throw new InvalidOperationException("You can't delete someone else Currency.");
			}

			currency.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);
		}

		/// <summary>
		/// Returns collection of User's currencies with props: Id and Name.
		/// </summary>
		public async Task<IEnumerable<CurrencyViewModel>> UserCurrencies(string userId)
		{
			string cacheKey = CacheKeyValue + userId;

			var currencies = await memoryCache.GetOrCreateAsync<IEnumerable<CurrencyViewModel>>(cacheKey, async cacheEntry =>
			{
				cacheEntry.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.Currencies
					.Where(c => (c.UserId == null || c.UserId == userId) && !c.IsDeleted)
					.Select(c => mapper.Map<CurrencyViewModel>(c))
					.ToArrayAsync();
			});

			return currencies;
		}
	}
}
