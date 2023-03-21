using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Categories.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize(Roles = UserRoleName)]
	[Route("api/categories")]
	[ApiController]
	public class CategoryApiController : ControllerBase
	{
		private readonly ICategoryService categoryService;

		public CategoryApiController(ICategoryService categoryService)
		{
			this.categoryService = categoryService;
		}

		//[HttpGet("{id}")]
		//public async Task<ActionResult<CategoryViewModel>> GetCategory(string id)
		//{
		//	if (!User.Identity?.IsAuthenticated ?? false)
		//		return Unauthorized();

		//	try
		//	{
		//		return await categoryService.GetCategoryViewModel(id, User.Id());
		//	}
		//	catch (InvalidOperationException)
		//	{
		//		return BadRequest();
		//	}
		//}

		[HttpPost]
		public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryInputModel inputModel)
		{
			try
			{
				CategoryViewModel viewModel = await categoryService
					.CreateCategory(User.Id(), inputModel.Name.Trim());

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
