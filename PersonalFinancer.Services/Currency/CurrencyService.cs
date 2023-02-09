using Microsoft.EntityFrameworkCore;
using PersonalFinancer.Services.Currency.Models;
using PersonalFinancer.Web.Data;

namespace PersonalFinancer.Services.Currency
{
	public class CurrencyService : ICurrencyService
	{
		private readonly PersonalFinancerDbContext data;

		public CurrencyService(PersonalFinancerDbContext data)
			=> this.data = data;

		public async Task<IEnumerable<CurrencyViewModel>> AllCurrencies(string userId)
		{
			return await data.Currencies
				.Where(c => c.UserId == null || c.UserId == userId)
				.Select(c => new CurrencyViewModel
				{
					Id= c.Id,
					Name = c.Name
				})
				.ToArrayAsync();
		}
	}
}
