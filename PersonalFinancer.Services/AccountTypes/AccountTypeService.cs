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

		public async Task<IEnumerable<AccountTypeViewModel>> GetUserAccountTypesViewModel(string userId)
		{
			string cacheKey = CacheKeyValue + userId;

			var accountTypes = await memoryCache.GetOrCreateAsync<IEnumerable<AccountTypeViewModel>>(cacheKey, async cacheEntry =>
			{
				cacheEntry.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.AccountTypes
					.Where(a => a.UserId == userId && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Select(a => mapper.Map<AccountTypeViewModel>(a))
					.ToArrayAsync();
			});

			return accountTypes;
		}

		/// <summary>
		/// Throws ArgumentException if given name already exists or name length is invalid.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task CreateAccountType(string userId, AccountTypeViewModel model)
		{
			AccountType? accountType = await data.AccountTypes
				.FirstOrDefaultAsync(c => c.Name == model.Name && c.UserId == userId);

			if (accountType != null)
			{
				if (accountType.IsDeleted == false)
				{
					throw new ArgumentException("Account Type with the same name exist!");
				}

				accountType.IsDeleted = false;
				accountType.Name = model.Name.Trim();
			}
			else
			{
				if (model.Name.Length < AccountTypeNameMinLength || model.Name.Length > AccountTypeNameMaxLength)
				{
					throw new ArgumentException($"Account Type name must be between {AccountTypeNameMinLength} and {AccountTypeNameMaxLength} characters long.");
				}

				accountType = new AccountType
				{
					Name = model.Name,
					UserId = userId
				};

				data.AccountTypes.Add(accountType);
			}

			await data.SaveChangesAsync();

			model.UserId = userId;
			model.Id = accountType.Id;

			memoryCache.Remove(CacheKeyValue + userId);
		}

		/// <summary>
		/// Throws exception when Account Type does not exist or User is not owner.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccountType(Guid accountTypeId, string userId)
		{
			AccountType? accountType = await data.AccountTypes.FindAsync(accountTypeId);

			if (accountType == null)
			{
				throw new InvalidOperationException("Account Type does not exist.");
			}

			if (accountType.UserId != userId)
			{
				throw new InvalidOperationException("You can't delete someone else Account Type.");
			}

			accountType.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);
		}
	}
}
