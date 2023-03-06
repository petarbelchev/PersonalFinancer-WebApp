namespace PersonalFinancer.Services.Currency
{
	using Microsoft.EntityFrameworkCore;
	using AutoMapper;

	using Data;
	using Models;
	using PersonalFinancer.Data.Models;

	public class CurrencyService : ICurrencyService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;

		public CurrencyService(
			PersonalFinancerDbContext data,
			IMapper mapper)
		{
			this.data = data;
			this.mapper = mapper;
		}

		/// <summary>
		/// Creates new Currency with given Name. If you try to create a new Currency with name that other Currency have, throws exception.
		/// </summary>
		/// <returns>View Model with Id, Name and User Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CurrencyViewModel> CreateCurrency(string userId, string currencyName)
		{
			Currency? currency = await data.Currencies
				.FirstOrDefaultAsync(c => c.Name == currencyName && c.UserId == userId);

			if (currency != null)
			{
				if (currency.IsDeleted == false)
				{
					throw new InvalidOperationException("Account Type with the same name exist!");
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

			//memoryCache.Remove(CacheKeyValue + userId);

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

			//memoryCache.Remove(CacheKeyValue + userId);
		}

		/// <summary>
		/// Returns collection of User's currencies with props: Id and Name.
		/// </summary>
		public async Task<IEnumerable<CurrencyViewModel>> UserCurrencies(string userId)
		{
			return await data.Currencies
				.Where(c => c.UserId == null || c.UserId == userId)
				.Select(c => mapper.Map<CurrencyViewModel>(c))
				.ToArrayAsync();
		}
	}
}
