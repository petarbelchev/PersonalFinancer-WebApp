using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Categories.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize]
	[Route("api/categories")]
	[ApiController]
	public class CategoryApiController : ControllerBase
	{
		private readonly ICategoryService categoryService;

		public CategoryApiController(ICategoryService categoryService)
			=> this.categoryService = categoryService;

		[HttpPost]
		public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryInputModel inputModel)
		{
			try
			{
				CategoryViewModel viewModel = 
					await categoryService.CreateCategory(inputModel);

				return Created(string.Empty, viewModel);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteCategory(string id)
		{
			try
			{
				if (User.IsAdmin())
					await categoryService.DeleteCategory(id);
				else
					await categoryService.DeleteCategory(id, User.Id());

				return NoContent();
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
