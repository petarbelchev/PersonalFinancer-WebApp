using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Currencies.Models;
using static PersonalFinancer.Data.Constants;

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
		public async Task<CurrencyOutputDTO> CreateCurrency(CurrencyInputDTO model)
		{
			Currency? currency = await data.Currencies
				.FirstOrDefaultAsync(c => c.Name == model.Name && c.OwnerId == model.OwnerId);

			if (currency != null)
			{
				if (currency.IsDeleted == false)
					throw new ArgumentException("Currency with the same name exist!");

				currency.IsDeleted = false;
				currency.Name = model.Name.Trim();
			}
			else
			{
				currency = new Currency
				{
					Id = Guid.NewGuid().ToString(),
					Name = model.Name.Trim(),
					OwnerId = model.OwnerId
				};

				await data.Currencies.AddAsync(currency);
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.CurrencyCacheKeyValue + model.OwnerId);

			return mapper.Map<CurrencyOutputDTO>(currency);
		}

		/// <summary>
		/// Throws InvalidOperationException when Currency does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteCurrency(string currencyId, string? ownerId = null)
		{
			Currency? currency = await data.Currencies.FindAsync(currencyId);

			if (currency == null)
				throw new InvalidOperationException("Currency does not exist.");

			if (ownerId != null && currency.OwnerId != ownerId)
				throw new ArgumentException("Can't delete someone else Currency.");

			currency.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.CurrencyCacheKeyValue + ownerId);
		}
	}
}
