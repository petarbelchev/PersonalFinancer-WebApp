using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Categories.Models;
using PersonalFinancer.Services.ModelsState;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
    [Authorize]
	[Route("api/categories")]
	[ApiController]
	public class CategoriesApiController : ControllerBase
	{
		private readonly ICategoryService categoryService;
		private readonly IModelStateService modelStateService;

		public CategoriesApiController(
			ICategoryService categoryService, 
			IModelStateService modelStateService)
		{
			this.categoryService = categoryService;
			this.modelStateService = modelStateService;
		}

		[HttpPost]
		public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(modelStateService.GetErrors(ModelState.Values));

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
