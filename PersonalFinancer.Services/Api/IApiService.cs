namespace PersonalFinancer.Services.Api
{
    using PersonalFinancer.Services.Api.Models;

    public interface IApiService<T>
	{
		/// <exception cref="ArgumentException">When try to create entity with existing name.</exception>
		Task<ApiEntityDTO> CreateEntityAsync(string name, Guid ownerId);

		/// <exception cref="ArgumentException">When the user is not owner or administrator.</exception>
		/// <exception cref="InvalidOperationException">When the entity does not exist.</exception>
		Task DeleteEntityAsync(Guid entityId, Guid userId, bool isUserAdmin);
    }
}
