namespace PersonalFinancer.Services.ApiService
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Contracts;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.ApiService.Models;
    using static PersonalFinancer.Data.Constants;

    public class ApiService<T> : IApiService<T> where T : ApiEntity, new()
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

        /// <summary>
        /// Throws ArgumentException if you try to create Entity with existing name.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ApiOutputServiceModel> CreateEntityAsync(string name, Guid ownerId)
        {
            T? entity = await this.repo.All().FirstOrDefaultAsync(
                x => x.Name == name && x.OwnerId == ownerId);

            if (entity != null)
            {
                if (entity.IsDeleted == false)
                    throw new ArgumentException("Entity with the same name exist.");

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

            this.ClearCache(entity.GetType().Name, ownerId);

            ApiOutputServiceModel outputModel = this.mapper.Map<ApiOutputServiceModel>(entity);

            return outputModel;
        }

        /// <summary>
        /// Throws InvalidOperationException when Entity does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task DeleteEntityAsync(Guid entityId, Guid userId, bool isUserAdmin)
        {
            T? entity = await this.repo.FindAsync(entityId) 
                ?? throw new InvalidOperationException("Entity does not exist.");
            
            if (isUserAdmin)
                userId = entity.OwnerId;

            if (entity.OwnerId != userId)
                throw new ArgumentException("Unauthorized.");

            entity.IsDeleted = true;

            await this.repo.SaveChangesAsync();

            this.ClearCache(entity.GetType().Name, userId);
        }

        private void ClearCache(string typeName, Guid ownerId)
        {
            if (typeName == nameof(AccountType))
                this.memoryCache.Remove(AccountTypeConstants.AccTypeCacheKeyValue + ownerId);
            else if (typeName == nameof(Category))
                this.memoryCache.Remove(CategoryConstants.CategoryCacheKeyValue + ownerId);
            else if (typeName == nameof(Currency))
                this.memoryCache.Remove(CurrencyConstants.CurrencyCacheKeyValue + ownerId);
        }
    }
}
