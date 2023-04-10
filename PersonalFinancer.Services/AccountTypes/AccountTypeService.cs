using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.Shared.Models;
using static PersonalFinancer.Data.Constants;

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
		/// Throws ArgumentException if given name already exists.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task<AccountTypeViewModel> CreateAccountType(AccountTypeInputModel model)
		{
			AccountType? accountType = await data.AccountTypes
				.FirstOrDefaultAsync(at => at.Name == model.Name && at.OwnerId == model.OwnerId);

			if (accountType != null)
			{
				if (accountType.IsDeleted == false)
					throw new ArgumentException("Account Type with the same name exist.");

				accountType.IsDeleted = false;
				accountType.Name = model.Name.Trim();
			}
			else
			{
				accountType = new AccountType
				{
					Id = Guid.NewGuid().ToString(),
					Name = model.Name.Trim(),
					OwnerId = model.OwnerId
				};

				await data.AccountTypes.AddAsync(accountType);
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.AccTypeCacheKeyValue + model.OwnerId);

			return mapper.Map<AccountTypeViewModel>(accountType);
		}
		
		/// <summary>
		/// Throws exception when Account Type does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccountType(string accountTypeId, string? ownerId = null)
		{
			AccountType? accountType = await data.AccountTypes.FindAsync(accountTypeId);

			if (accountType == null)
				throw new InvalidOperationException("Account Type does not exist.");

			if (ownerId != null && accountType.OwnerId != ownerId)
				throw new ArgumentException("Can't delete someone else Account Type.");

			accountType.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.AccTypeCacheKeyValue + ownerId);
		}
	}
}
