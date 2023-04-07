using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Categories.Models;
using PersonalFinancer.Services.ModelsState;
using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Models.Shared;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize]
	[Route("api/categories")]
	[ApiController]
	public class CategoriesApiController : ControllerBase
	{
		private readonly ICategoryService categoryService;
		private readonly IModelStateService modelStateService;
		private readonly IMapper mapper;

		public CategoriesApiController(
			ICategoryService categoryService,
			IModelStateService modelStateService,
			IMapper mapper)
		{
			this.categoryService = categoryService;
			this.modelStateService = modelStateService;
			this.mapper = mapper;
		}

		[HttpPost]
		public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(modelStateService.GetErrors(ModelState.Values));

			try
			{
				var inputDTO = mapper.Map<CategoryInputDTO>(inputModel);
				CategoryOutputDTO categoryOutputDTO =
					await categoryService.CreateCategory(inputDTO);
				var viewModel = mapper.Map<CategoryViewModel>(categoryOutputDTO);

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
