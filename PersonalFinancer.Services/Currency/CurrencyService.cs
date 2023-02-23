namespace PersonalFinancer.Services.Currency
{
	using Microsoft.EntityFrameworkCore;
	using AutoMapper;

	using Data;
	using Models;

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
