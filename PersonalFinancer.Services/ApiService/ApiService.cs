using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Data.Models.Contracts;
using PersonalFinancer.Data.Repositories;
using PersonalFinancer.Services.ApiService.Models;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Services.ApiService
{
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
        public async Task<ApiOutputServiceModel> CreateEntity(IApiInputServiceModel model)
        {
            T? entity = await repo.All().FirstOrDefaultAsync(
                x => x.Name == model.Name && x.OwnerId == model.OwnerId);

            if (entity != null)
            {
                if (entity.IsDeleted == false)
                    throw new ArgumentException("Entity with the same name exist.");

                entity.IsDeleted = false;
                entity.Name = model.Name.Trim();
            }
            else
            {
                entity = new T
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name.Trim(),
                    OwnerId = model.OwnerId
                };

                await repo.AddAsync(entity);
            }

            await repo.SaveChangesAsync();

            ClearCache(entity.GetType().Name, entity.OwnerId);

            var outputModel = mapper.Map<ApiOutputServiceModel>(entity);

            return outputModel;
        }

        /// <summary>
        /// Throws InvalidOperationException when Entity does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task DeleteEntity(string entityId, string userId, bool isUserAdmin)
        {
            T? entity = await repo.FindAsync(entityId);

            if (entity == null)
                throw new InvalidOperationException("Entity does not exist.");

            if (isUserAdmin)
                userId = entity.OwnerId;

            if (entity.OwnerId != userId)
                throw new ArgumentException("Unauthorized.");

            entity.IsDeleted = true;

            await repo.SaveChangesAsync();

            ClearCache(entity.GetType().Name, userId);
        }

        private void ClearCache(string typeName, string ownerId)
        {
            if (typeName == nameof(AccountType))
                memoryCache.Remove(AccountTypeConstants.AccTypeCacheKeyValue + ownerId);
            else if (typeName == nameof(Category))
                memoryCache.Remove(CategoryConstants.CategoryCacheKeyValue + ownerId);
            else if (typeName == nameof(Currency))
                memoryCache.Remove(CurrencyConstants.CurrencyCacheKeyValue + ownerId);
        }
    }
}
