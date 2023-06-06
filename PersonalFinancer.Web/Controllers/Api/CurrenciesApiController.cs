using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.ApiService;
using PersonalFinancer.Services.ApiService.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure.Extensions;

namespace PersonalFinancer.Web.Controllers.Api
{
    [Route("api/currencies")]
	[ApiController]
	public class CurrenciesApiController : BaseApiController
	{
		private readonly IApiService<Currency> apiService;

		public CurrenciesApiController(IApiService<Currency> apiService)
			=> this.apiService = apiService;

		[HttpPost]
		public async Task<ActionResult> Create(CurrencyInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(GetErrors(ModelState.Values));

			try
			{
				ApiOutputServiceModel model =
					await apiService.CreateEntity(inputModel);

				return Created(string.Empty, model);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(string id)
		{
			try
			{
				await apiService.DeleteEntity(id, User.Id(), User.IsAdmin());

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