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
		/// Throws InvalidOperationException when Category does not exist or User is not owner.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteCategory(Guid categoryId, string userId)
		{
			Category? category = await data.Categories.FindAsync(categoryId);

			if (category == null)
			{
				throw new InvalidOperationException("Category does not exist.");
			}

			if (category.UserId != userId)
			{
				throw new InvalidOperationException("You can't delete someone else category.");
			}

			category.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);
		}
		
		/// <summary>
		/// Throws InvalidOperationException when Category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CategoryViewModel> GetCategoryViewModel(Guid categoryId)
		{
			return await data.Categories
				.Where(c => c.Id == categoryId)
				.Select(c => mapper.Map<CategoryViewModel>(c))
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<Guid> GetCategoryIdByName(string categoryName)
		{
			Category category = await data.Categories.FirstAsync(c => c.Name == categoryName);

			return category.Id;
		}

		/// <summary>
		/// Throws ArgumentException if try to create Category with existing or invalid name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task CreateCategory(string userId, CategoryViewModel model)
		{
			Category? category = await data.Categories
				.FirstOrDefaultAsync(c => c.Name == model.Name && c.UserId == userId);

			if (category != null)
			{
				if (category.IsDeleted == false)
				{
					throw new ArgumentException("Category with the same name exist!");
				}

				category.IsDeleted = false;
				category.Name = model.Name.Trim();
			}
			else
			{
				if (model.Name.Length < CategoryNameMinLength || model.Name.Length > CategoryNameMaxLength)
				{
					throw new ArgumentException($"Category name must be between {CategoryNameMinLength} and {CategoryNameMaxLength} characters long.");
				}

				category = new Category
				{
					Name = model.Name,
					UserId = userId
				};

				data.Categories.Add(category);
			}

			await data.SaveChangesAsync();

			model.Id = category.Id;
			model.UserId = category.UserId;

			memoryCache.Remove(CacheKeyValue + userId);
		}
		
		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<bool> IsInitialBalance(Guid categoryId)
		{
			Category? category = await data.Categories.FindAsync(categoryId);

			if (category == null)
			{
				throw new InvalidOperationException("Category does not exist.");
			}

			return category.Name == CategoryInitialBalanceName;
		}

		public async Task<IEnumerable<CategoryViewModel>> GetUserCategories(string userId)
		{
			string cacheKey = CacheKeyValue + userId;

			var categories = await memoryCache.GetOrCreateAsync<IEnumerable<CategoryViewModel>>(cacheKey, async cacheEntity =>
			{
				cacheEntity.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.Categories
					.Where(c => c.UserId == userId && !c.IsDeleted)
					.OrderBy(c => c.Name)
					.Select(c => mapper.Map<CategoryViewModel>(c))
					.ToArrayAsync();
			});

			return categories;
		}
	}
}
