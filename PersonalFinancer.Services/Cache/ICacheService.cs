namespace PersonalFinancer.Services.Cache
{
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Services.Shared.Contracts;

	public interface ICacheService<T> where T : BaseApiEntity, new()
	{
		Task<IEnumerable<TResult>> GetValues<TResult>(Guid userId, bool withDeleted)
			where TResult : BaseCacheableServiceModel, new();

		void RemoveValues(Guid userId, bool notDeleted, bool deleted);
	}
}
