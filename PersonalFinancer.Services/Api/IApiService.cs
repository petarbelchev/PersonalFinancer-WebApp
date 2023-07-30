namespace PersonalFinancer.Services.Api
{
    using PersonalFinancer.Services.Api.Models;

    public interface IApiService<T>
	{
		/// <exception cref="ArgumentException">When try to create entity with existing name.</exception>
		Task<ApiEntityDTO> CreateEntityAsync(string name, Guid ownerId);

		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When the entity does not exist.</exception>
		Task DeleteEntityAsync(Guid entityId, Guid userId, bool isUserAdmin);
    }
}
