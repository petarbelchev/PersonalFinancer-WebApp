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
		/// Throws ArgumentException if given name exists or lenght is invalid.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task CreateCurrency(string userId, CurrencyViewModel model)
		{
			Currency? currency = await data.Currencies
				.FirstOrDefaultAsync(c => c.Name == model.Name && c.UserId == userId);

			if (currency != null)
			{
				if (currency.IsDeleted == false)
				{
					throw new ArgumentException("Currency with the same name exist!");
				}

				currency.IsDeleted = false;
				currency.Name = model.Name.Trim();
			}
			else
			{
				if (model.Name.Length < CurrencyNameMinLength || model.Name.Length > CurrencyNameMaxLength)
				{
					throw new ArgumentException($"Currency name must be between {CurrencyNameMinLength} and {CurrencyNameMaxLength} characters long.");
				}

				currency = new Currency
				{
					Name = model.Name,
					UserId = userId
				};

				data.Currencies.Add(currency);
			}

			await data.SaveChangesAsync();

			model.Id = currency.Id;
			model.UserId = currency.UserId;

			memoryCache.Remove(CacheKeyValue + userId);
		}

		/// <summary>
		/// Throws InvalidOperationException when Currency does not exist or User is not owner.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteCurrency(Guid currencyId, string userId)
		{
			Currency? currency = await data.Currencies.FindAsync(currencyId);

			if (currency == null)
			{
				throw new InvalidOperationException("Currency does not exist.");
			}

			if (currency.UserId != userId)
			{
				throw new InvalidOperationException("Can't delete someone else Currency.");
			}

			currency.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);
		}

		public async Task<IEnumerable<CurrencyViewModel>> GetUserCurrencies(string userId)
		{
			string cacheKey = CacheKeyValue + userId;

			var currencies = await memoryCache.GetOrCreateAsync<IEnumerable<CurrencyViewModel>>(cacheKey, async cacheEntry =>
			{
				cacheEntry.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.Currencies
					.Where(c => (c.UserId == null || c.UserId == userId) && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => mapper.Map<CurrencyViewModel>(c))
					.ToArrayAsync();
			});

			return currencies;
		}
	}
}
