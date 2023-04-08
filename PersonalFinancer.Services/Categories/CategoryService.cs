namespace PersonalFinancer.Services.Categories
{
	using AutoMapper;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;

	using Data;
	using Data.Models;
	using static Data.Constants.CategoryConstants;

	using Services.Categories.Models;

	public class CategoryService : ICategoryService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMemoryCache memoryCache;
		private readonly IMapper mapper;

		public CategoryService(
			PersonalFinancerDbContext context,
			IMemoryCache memoryCache,
			IMapper mapper)
		{
			this.data = context;
			this.memoryCache = memoryCache;
			this.mapper = mapper;
		}

		/// <summary>
		/// Throws ArgumentException if try to create Category with existing name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task<CategoryOutputDTO> CreateCategory(CategoryInputDTO inputDTO)
		{
			Category? category = await data.Categories
				.FirstOrDefaultAsync(c => c.Name == inputDTO.Name && c.OwnerId == inputDTO.OwnerId);

			if (category != null)
			{
				if (category.IsDeleted == false)
					throw new ArgumentException("Category with the same name exist!");

				category.IsDeleted = false;
				category.Name = inputDTO.Name.Trim();
			}
			else
			{
				category = new Category
				{
					Id = Guid.NewGuid().ToString(),
					Name = inputDTO.Name.Trim(),
					OwnerId = inputDTO.OwnerId
				};

				data.Categories.Add(category);
			}
			await data.SaveChangesAsync();

			memoryCache.Remove(CategoryCacheKeyValue + inputDTO.OwnerId);

			return mapper.Map<CategoryOutputDTO>(category);
		}

		/// <summary>
		/// Throws InvalidOperationException when Category does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteCategory(string categoryId, string? ownerId = null)
		{
			Category? category = await data.Categories.FindAsync(categoryId);

			if (category == null)
				throw new InvalidOperationException("Category does not exist.");

			if (ownerId != null && category.OwnerId != ownerId)
				throw new ArgumentException("Can't delete someone else category.");

			category.IsDeleted = true;
			await data.SaveChangesAsync();

			memoryCache.Remove(CategoryCacheKeyValue + ownerId);
		}
	}
}
