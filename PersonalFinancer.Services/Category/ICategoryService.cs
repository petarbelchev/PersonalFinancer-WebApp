namespace PersonalFinancer.Services.Category
{
	using Models;

	public interface ICategoryService
	{
		Task<IEnumerable<CategoryViewModel>> All();

		Task<int> CategoryIdByName(string name);

		Task<CategoryViewModel> CategoryById(int id);
	}
}
