namespace PersonalFinancer.Services.Categories
{
	using Services.Categories.Models;
	
	public interface ICategoryService
	{
		/// <summary>
		/// Throws ArgumentException if try to create Category with existing name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task<CategoryOutputDTO> CreateCategory(CategoryInputDTO inputDTO);

		/// <summary>
		/// Throws InvalidOperationException when Category does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteCategory(string categoryId, string? ownerId = null);
	}
}
