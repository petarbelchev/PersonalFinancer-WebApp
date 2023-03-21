using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.AccountTypes.Models;
using static PersonalFinancer.Data.Constants.AccountTypeConstants;

namespace PersonalFinancer.Services.AccountTypes
{
	public class AccountTypeService : IAccountTypeService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public AccountTypeService(
			PersonalFinancerDbContext data,
			IMapper mapper,
			IMemoryCache memoryCache)
		{
			this.data = data;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
		}

		public async Task<IEnumerable<AccountTypeViewModel>> GetUserAccountTypesViewModel(string ownerId)
		{
			string cacheKey = CacheKeyValue + ownerId;

			var accountTypes = await memoryCache.GetOrCreateAsync<IEnumerable<AccountTypeViewModel>>(cacheKey, async cacheEntry =>
			{
				cacheEntry.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.AccountTypes
					.Where(a => a.OwnerId == ownerId && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Select(a => mapper.Map<AccountTypeViewModel>(a))
					.ToArrayAsync();
			});

			return accountTypes;
		}

		/// <summary>
		/// Throws ArgumentException if given name already exists.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task<AccountTypeViewModel> CreateAccountType(string userId, string accountTypeName)
		{
			AccountType? accountType = await data.AccountTypes
				.FirstOrDefaultAsync(at => at.Name == accountTypeName && at.OwnerId == userId);

			if (accountType != null)
			{
				if (accountType.IsDeleted == false)
					throw new ArgumentException("Account Type with the same name exist!");

				accountType.IsDeleted = false;
				accountType.Name = accountTypeName;
			}
			else
			{
				accountType = new AccountType
				{
					Id = Guid.NewGuid().ToString(),
					Name = accountTypeName,
					OwnerId = userId
				};

				data.AccountTypes.Add(accountType);
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);

			return mapper.Map<AccountTypeViewModel>(accountType);
		}

		/// <summary>
		/// Throws exception when Account Type does not exist
		/// and ArgumentException when User is not owner.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccountType(string accountTypeId, string userId)
		{
			AccountType? accountType = await data.AccountTypes.FindAsync(accountTypeId);

			if (accountType == null)
				throw new InvalidOperationException("Account Type does not exist.");

			if (accountType.OwnerId != userId)
				throw new ArgumentException("Can't delete someone else Account Type.");

			accountType.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);
		}
	}
}
