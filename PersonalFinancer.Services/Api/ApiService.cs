namespace PersonalFinancer.Services.Api
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Distributed;
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
		private readonly IDistributedCache cache;

		public ApiService(
            IEfRepository<T> repo,
            IMapper mapper,
			IDistributedCache cache)
        {
            this.repo = repo;
            this.mapper = mapper;
			this.cache = cache;
		}

        public async Task<ApiEntityDTO> CreateEntityAsync(string name, Guid ownerId)
        {
            string trimmedName = name.Trim();

            T? entity = await this.repo.All()
                .FirstOrDefaultAsync(x => x.Name.ToLower() == trimmedName.ToLower() && 
                                          x.OwnerId == ownerId);

            if (entity != null)
            {
                if (entity.IsDeleted == false)
                    throw new ArgumentException(ExceptionMessages.ExistingEntityName);

                entity.IsDeleted = false;
                entity.Name = trimmedName;
            }
            else
            {
                entity = new T
                {
                    Name = trimmedName,
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
            
            if (!isUserAdmin && entity.OwnerId != userId)
                throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);

            entity.IsDeleted = true;
            await this.repo.SaveChangesAsync();
            this.RemoveCache(entity.OwnerId);
		}

		private void RemoveCache(Guid userId)
		{
			if (typeof(T) == typeof(Category))
			{
				this.cache.Remove(AccountsAndCategoriesKey + userId);
			}
			else if (typeof(T) == typeof(AccountType) || typeof(T) == typeof(Currency))
			{
				this.cache.Remove(AccountTypesAndCurrenciesKey + userId);
			}
		}
	}
}
