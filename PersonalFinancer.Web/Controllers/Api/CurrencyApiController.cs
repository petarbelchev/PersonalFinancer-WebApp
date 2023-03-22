using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Currencies;
using PersonalFinancer.Services.Currencies.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize]
	[Route("api/currencies")]
	[ApiController]
	public class CurrencyApiController : ControllerBase
	{
		private readonly ICurrencyService currencyService;

		public CurrencyApiController(ICurrencyService currencyService)
			=> this.currencyService = currencyService;

		[HttpPost]
		public async Task<ActionResult<CurrencyViewModel>> Create(CurrencyInputModel inputModel)
		{
			try
			{
				CurrencyViewModel viewModel = 
					await currencyService.CreateCurrency(inputModel);

				return Created(string.Empty, viewModel);
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
				await currencyService.DeleteCurrency(id, User.Id());

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