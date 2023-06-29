namespace PersonalFinancer.Services.Api
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models.Contracts;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Api.Models;

    public class ApiService<T> : IApiService<T> where T : BaseApiEntity, new()
    {
        private readonly IEfRepository<T> repo;
        private readonly IMapper mapper;

        public ApiService(
            IEfRepository<T> repo,
            IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
		}

        /// <summary>
        /// Throws Argument Exception if you try to create entity with existing name.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ApiEntityDTO> CreateEntityAsync(string name, Guid ownerId)
        {
            T? entity = await this.repo.All().FirstOrDefaultAsync(
                x => x.Name == name && x.OwnerId == ownerId);

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

            ApiEntityDTO outputModel = this.mapper.Map<ApiEntityDTO>(entity);

            return outputModel;
        }

        /// <summary>
        /// Throws Invalid Operation Exception when the entity does not exist
        /// and Argument Exception when the user is not owner or administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task DeleteEntityAsync(Guid entityId, Guid userId, bool isUserAdmin)
        {
            T? entity = await this.repo.FindAsync(entityId) 
                ?? throw new InvalidOperationException(ExceptionMessages.EntityDoesNotExist);
            
            if (isUserAdmin)
                userId = entity.OwnerId;

            if (entity.OwnerId != userId)
                throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

            entity.IsDeleted = true;

            await this.repo.SaveChangesAsync();
        }
    }
}
