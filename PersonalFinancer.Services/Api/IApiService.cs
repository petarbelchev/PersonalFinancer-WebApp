namespace PersonalFinancer.Services.Api
{
    using PersonalFinancer.Services.Api.Models;

    public interface IApiService<T>
	{
		/// <summary>
		/// Throws Argument Exception if you try to create entity with existing name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task<ApiEntityDTO> CreateEntityAsync(string name, Guid ownerId);

		/// <summary>
		/// Throws Invalid Operation Exception when the entity does not exist
		/// and Argument Exception when the user is not owner or administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteEntityAsync(Guid entityId, Guid userId, bool isUserAdmin);
    }
}
