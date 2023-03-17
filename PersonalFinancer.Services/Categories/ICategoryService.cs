using PersonalFinancer.Services.Categories.Models;

namespace PersonalFinancer.Services.Categories
{
	public interface ICategoryService
	{
		/// <summary>
		/// Throws InvalidOperationException when Category does not exist or User is not owner.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteCategory(string categoryId, string userId);
		
		/// <summary>
		/// Throws InvalidOperationException when Category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<CategoryViewModel> GetCategoryViewModel(string categoryId);

		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> GetCategoryIdByName(string categoryName);
		
		/// <summary>
		/// Throws ArgumentException if try to create Category with existing or invalid name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task CreateCategory(string userId, CategoryViewModel model);
		
		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<bool> IsInitialBalance(string categoryId);

		Task<IEnumerable<CategoryViewModel>> GetUserCategories(string userId);
	}
}
