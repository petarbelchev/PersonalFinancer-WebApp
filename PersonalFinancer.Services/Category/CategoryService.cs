using Microsoft.EntityFrameworkCore;
using PersonalFinancer.Services.Category.Models;
using PersonalFinancer.Web.Data;

namespace PersonalFinancer.Services.Category
{
	public class CategoryService : ICategoryService
	{
		private readonly PersonalFinancerDbContext data;

		public CategoryService(PersonalFinancerDbContext context)
			=> this.data = context;

		public async Task<IEnumerable<CategoryViewModel>> All()
		{
			return await data.Categories
				.Skip(1) // Skiped the first category which is Initial Balance
				.Select(c => new CategoryViewModel()
				{
					Id = c.Id,
					Name = c.Name
				})
				.ToArrayAsync();
		}
	}
}
