namespace PersonalFinancer.Services.Api
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Api.Models;
	using static PersonalFinancer.Common.Constants.CacheConstants;

	public class ApiService<T> : IApiService<T> where T : BaseApiEntity, new()
    {
        private readonly IEfRepository<T> repo;
        private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public ApiService(
            IEfRepository<T> repo,
            IMapper mapper,
			IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.mapper = mapper;
			this.memoryCache = memoryCache;
		}

        public async Task<ApiEntityDTO> CreateEntityAsync(string name, Guid ownerId)
        {
            T? entity = await this.repo.All()
                .FirstOrDefaultAsync(x => x.Name == name && x.OwnerId == ownerId);

            if (entity != null)
            {
                if (entity.IsDeleted == false)
                    throw new ArgumentException(ExceptionMessages.ExistingEntityName);

                entity.IsDeleted = false;
                entity.Name = name.Trim();
            }
            else
            {
                entity = new T
                {
                    Name = name.Trim(),
                    OwnerId = ownerId
                };

                await this.repo.AddAsync(entity);
            }

            await this.repo.SaveChangesAsync();
            this.RemoveCache(ownerId);
            ApiEntityDTO outputModel = this.mapper.Map<ApiEntityDTO>(entity);

            return outputModel;
        }

        public async Task DeleteEntityAsync(Guid entityId, Guid userId, bool isUserAdmin)
        {
            T? entity = await this.repo.FindAsync(entityId) ?? 
                throw new InvalidOperationException(ExceptionMessages.EntityDoesNotExist);
            
            if (isUserAdmin)
                userId = entity.OwnerId;

            if (entity.OwnerId != userId)
                throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

            entity.IsDeleted = true;
            await this.repo.SaveChangesAsync();
            this.RemoveCache(userId);
		}

		private void RemoveCache(Guid userId)
		{
			if (typeof(T) == typeof(Category))
			{
				this.memoryCache.Remove(AccountsAndCategoriesKey + userId);
			}
			else if (typeof(T) == typeof(AccountType) || typeof(T) == typeof(Currency))
			{
				this.memoryCache.Remove(AccountTypesAndCurrenciesKey + userId);
			}
		}
	}
}
