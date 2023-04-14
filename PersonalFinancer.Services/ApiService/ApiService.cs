namespace PersonalFinancer.Services.ApiService
{
    using AutoMapper;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    using Data;
    using Data.Models;
    using static Data.Constants;

    using PersonalFinancer.Services.ApiService.Models;

    public class ApiService<T> where T : ApiEntity, new()
    {
        private readonly PersonalFinancerDbContext context;
        private readonly DbSet<T> dbSet;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public ApiService(
            PersonalFinancerDbContext context,
            IMapper mapper,
            IMemoryCache memoryCache)
        {
            this.context = context;
            dbSet = this.context.Set<T>();
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// Throws ArgumentException if you try to create Entity with existing name.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ApiOutputServiceModel> CreateEntity(ApiInputServiceModel model)
        {
            T? entity = await dbSet.FirstOrDefaultAsync(
                at => at.Name == model.Name && at.OwnerId == model.OwnerId);

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

                await context.Set<T>().AddAsync(entity);
            }

            await context.SaveChangesAsync();

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
            T? entity = await dbSet.FindAsync(entityId);

            if (entity == null)
                throw new InvalidOperationException("Entity does not exist.");

            if (isUserAdmin)
                userId = entity.OwnerId;

            if (entity.OwnerId != userId)
                throw new ArgumentException("Unauthorized.");

            entity.IsDeleted = true;

            await context.SaveChangesAsync();

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
