namespace PersonalFinancer.Services.Api
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
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
        /// Throws ArgumentException if you try to create Entity with existing name.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ApiEntityDTO> CreateEntityAsync(string name, Guid ownerId)
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

            ApiEntityDTO outputModel = this.mapper.Map<ApiEntityDTO>(entity);

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
        }
    }
}
