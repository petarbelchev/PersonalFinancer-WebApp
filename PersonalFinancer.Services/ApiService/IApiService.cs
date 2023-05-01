namespace PersonalFinancer.Services.ApiService
{
	using Services.ApiService.Models;

	public interface IApiService<T>
	{
		Task<ApiOutputServiceModel> CreateEntity(IApiInputServiceModel model);

		Task DeleteEntity(string entityId, string userId, bool isUserAdmin);
	}
}
