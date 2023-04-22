using PersonalFinancer.Services.ApiService.Models;

namespace PersonalFinancer.Services.ApiService
{
	public interface IApiService<T>
	{
		Task<ApiOutputServiceModel> CreateEntity(ApiInputServiceModel model);

		Task DeleteEntity(string entityId, string userId, bool isUserAdmin);
	}
}
