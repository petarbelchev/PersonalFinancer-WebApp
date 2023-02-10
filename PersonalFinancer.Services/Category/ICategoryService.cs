using PersonalFinancer.Services.Category.Models;

namespace PersonalFinancer.Services.Category
{
	public interface ICategoryService
	{
		Task<IEnumerable<CategoryViewModel>> All();

		Task<int> CategoryIdByName(string name);

		Task<CategoryViewModel> CategoryById(int id);
	}
}
