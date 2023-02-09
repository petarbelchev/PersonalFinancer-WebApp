using PersonalFinancer.Services.Category.Models;

namespace PersonalFinancer.Services.Category
{
	public interface ICategoryService
	{
		Task<IEnumerable<CategoryViewModel>> All();
	}
}
