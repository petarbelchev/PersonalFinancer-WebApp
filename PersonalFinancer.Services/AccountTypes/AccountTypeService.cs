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

		/// <summary>
		/// Returns collection of Account Types for the current user with Id and Name.
		/// </summary>
		public async Task<IEnumerable<AccountTypeViewModel>> AccountTypesViewModel(string userId)
		{
			string cacheKey = CacheKeyValue + userId;

			var accountTypes = await memoryCache.GetOrCreateAsync<IEnumerable<AccountTypeViewModel>>(cacheKey, async cacheEntry =>
			{
				cacheEntry.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.AccountTypes
					.Where(a => (a.UserId == null || a.UserId == userId) && !a.IsDeleted)
					.Select(a => mapper.Map<AccountTypeViewModel>(a))
					.ToArrayAsync();
			});

			return accountTypes;
		}

		/// <summary>
		/// Creates new Account Type with given Name. 
		/// If you try to create a new Account Type with name that other Account Type have, throws exception.
		/// </summary>
		/// <returns>View Model with Id, Name and User Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountTypeViewModel> CreateAccountType(string userId, string accountTypeName)
		{
			AccountType? accountType = await data.AccountTypes
				.FirstOrDefaultAsync(c =>
					c.Name == accountTypeName
					&& (c.UserId == userId || c.UserId == null));

			if (accountType != null)
			{
				if (accountType.IsDeleted == false)
				{
					throw new InvalidOperationException("Account Type with the same name exist!");
				}

				accountType.IsDeleted = false;
			}
			else
			{
				if (accountTypeName.Length < AccountTypeNameMinLength || accountTypeName.Length > AccountTypeNameMaxLength)
				{
					throw new InvalidOperationException($"Account Type name must be between {AccountTypeNameMinLength} and {AccountTypeNameMaxLength} characters long.");
				}

				accountType = new AccountType
				{
					Name = accountTypeName,
					UserId = userId
				};

				data.AccountTypes.Add(accountType);
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);

			return mapper.Map<AccountTypeViewModel>(accountType);
		}

		/// <summary>
		/// Delete Account Type with given Id or throws exception when Account Type does not exist or User is not owner.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccountType(Guid accountTypeId, string userId)
		{
			AccountType? accountType = await data.AccountTypes.FindAsync(accountTypeId);

			if (accountType == null)
			{
				throw new ArgumentNullException("Account Type does not exist.");
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
