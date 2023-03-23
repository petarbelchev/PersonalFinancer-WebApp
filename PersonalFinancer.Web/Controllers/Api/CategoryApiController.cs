using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
    [Authorize]
	[Route("api/categories")]
	[ApiController]
	public class CategoryApiController : ControllerBase
	{
		private readonly ITransactionsService transactionsService;

		public CategoryApiController(ITransactionsService transactionsService)
			=> this.transactionsService = transactionsService;

		[HttpPost]
		public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryInputModel inputModel)
		{
			try
			{
				CategoryViewModel viewModel = 
					await transactionsService.CreateCategory(inputModel);

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
					await transactionsService.DeleteCategory(id);
				else
					await transactionsService.DeleteCategory(id, User.Id());

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
