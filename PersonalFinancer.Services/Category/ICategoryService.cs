namespace PersonalFinancer.Services.Category
{
	using Models;

	public interface ICategoryService
	{
		Task<IEnumerable<CategoryViewModel>> All();

		Task<Guid> CategoryIdByName(string name);

		Task<CategoryViewModel> CategoryById(Guid id);

		Task<bool> IsInitialBalance(Guid id);
	}
}
