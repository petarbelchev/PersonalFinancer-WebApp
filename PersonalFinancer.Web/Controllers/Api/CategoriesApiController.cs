namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Categories;
	using Services.Categories.Models;

	using Web.Infrastructure;
	using Web.Models.Shared;

	[Authorize]
	[Route("api/categories")]
	[ApiController]
	public class CategoriesApiController : ControllerBase
	{
		private readonly ICategoryService categoryService;
		private readonly IControllerService controllerService;
		private readonly IMapper mapper;

		public CategoriesApiController(
			ICategoryService categoryService,
			IControllerService controllerService,
			IMapper mapper)
		{
			this.categoryService = categoryService;
			this.controllerService = controllerService;
			this.mapper = mapper;
		}

		[HttpPost]
		public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(controllerService.GetModelErrors(ModelState.Values));

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
