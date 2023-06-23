namespace PersonalFinancer.Services.Api
{
    using PersonalFinancer.Services.Api.Models;

    public interface IApiService<T>
    {
        /// <summary>
        /// Throws ArgumentException if you try to create Entity with existing name.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
		Task<ApiOutputServiceModel> CreateEntityAsync(string name, Guid ownerId);

        /// <summary>
        /// Throws InvalidOperationException when Entity does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task DeleteEntityAsync(Guid entityId, Guid userId, bool isUserAdmin);
    }
}
