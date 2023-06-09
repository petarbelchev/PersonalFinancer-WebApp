namespace PersonalFinancer.Services.ApiService
{
    using PersonalFinancer.Services.ApiService.Models;

    public interface IApiService<T>
    {
        /// <summary>
        /// Throws ArgumentException if you try to create Entity with existing name.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
		Task<ApiOutputServiceModel> CreateEntity(string name, Guid ownerId);

        /// <summary>
        /// Throws InvalidOperationException when Entity does not exist
        /// and ArgumentException when User is not owner or Administrator.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task DeleteEntity(Guid entityId, Guid userId, bool isUserAdmin);
    }
}
