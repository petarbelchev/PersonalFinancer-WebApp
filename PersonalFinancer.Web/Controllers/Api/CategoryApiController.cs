using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Categories.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Route("api/categories")]
	[ApiController]
	public class CategoryApiController : ControllerBase
	{
		private readonly ICategoryService categoryService;

		public CategoryApiController(ICategoryService categoryService)
		{
			this.categoryService = categoryService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<CategoryViewModel>> GetCategory(string id)
		{
			if (!User.Identity?.IsAuthenticated ?? false)
				return Unauthorized();

			try
			{
				return await categoryService.GetCategoryViewModel(id);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryViewModel model)
		{
			if (!User.Identity?.IsAuthenticated ?? false)
				return Unauthorized();

			try
			{
				await categoryService.CreateCategory(User.Id(), model);

				return Created(string.Empty, model);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteCategory(string id)
		{
			if (!User.Identity?.IsAuthenticated ?? false)
				return Unauthorized();

			try
			{
				await categoryService.DeleteCategory(id, User.Id());

				return NoContent();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
