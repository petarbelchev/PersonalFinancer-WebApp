using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Categories.Models;
using static PersonalFinancer.Data.Constants.CategoryConstants;

namespace PersonalFinancer.Services.Categories
{
	public class CategoryService : ICategoryService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public CategoryService(
			PersonalFinancerDbContext context,
			IMapper mapper,
			IMemoryCache memoryCache)
		{
			this.data = context;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
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

			memoryCache.Remove(CacheKeyValue + ownerId);
		}
		
		/// <summary>
		/// Throws InvalidOperationException when Category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CategoryViewModel> GetCategoryViewModel(string categoryId, string ownerId)
		{
			return await data.Categories
				.Where(c => c.Id == categoryId && c.OwnerId == ownerId)
				.Select(c => mapper.Map<CategoryViewModel>(c))
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> GetCategoryIdByName(string categoryName)
		{
			Category category = await data.Categories.FirstAsync(c => c.Name == categoryName);

			return category.Id;
		}

		/// <summary>
		/// Throws ArgumentException if try to create Category with existing name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task<CategoryViewModel> CreateCategory(CategoryInputModel model)
		{
			Category? category = await data.Categories
				.FirstOrDefaultAsync(c => c.Name == model.Name && c.OwnerId == model.OwnerId);

			if (category != null)
			{
				if (category.IsDeleted == false)
					throw new ArgumentException("Category with the same name exist!");

				category.IsDeleted = false;
				category.Name = model.Name.Trim();
			}
			else
			{
				category = new Category
				{
					Id = Guid.NewGuid().ToString(),
					Name = model.Name.Trim(),
					OwnerId = model.OwnerId
				};

				data.Categories.Add(category);
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + model.OwnerId);

			return mapper.Map<CategoryViewModel>(category);
		}
		
		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<bool> IsInitialBalance(string categoryId)
		{
			Category? category = await data.Categories.FindAsync(categoryId);

			if (category == null)
				throw new InvalidOperationException("Category does not exist.");

			return category.Name == CategoryInitialBalanceName;
		}

		public async Task<IEnumerable<CategoryViewModel>> GetUserCategories(string userId)
		{
			string cacheKey = CacheKeyValue + userId;

			var categories = await memoryCache.GetOrCreateAsync<IEnumerable<CategoryViewModel>>(cacheKey, async cacheEntity =>
			{
				cacheEntity.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.Categories
					.Where(c => c.OwnerId == userId && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => mapper.Map<CategoryViewModel>(c))
					.ToArrayAsync();
			});

			return categories;
		}
	}
}
