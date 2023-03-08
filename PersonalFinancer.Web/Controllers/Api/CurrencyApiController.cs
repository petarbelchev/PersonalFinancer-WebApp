using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Currencies;
using PersonalFinancer.Services.Currencies.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Route("api/currencies")]
	[ApiController]
	public class CurrencyApiController : ControllerBase
	{
		private readonly ICurrencyService currencyService;

		public CurrencyApiController(ICurrencyService currencyService)
		{
			this.currencyService = currencyService;
		}

		[HttpPost]
		public async Task<IActionResult> Create(CurrencyViewModel model)
		{
			if (!User.Identity?.IsAuthenticated ?? false)
			{
				return Unauthorized();
			}

			try
			{
				CurrencyViewModel currency = await currencyService.CreateCurrency(User.Id(), model.Name);

				return Created(string.Empty, currency);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(Guid id)
		{
			if (!User.Identity?.IsAuthenticated ?? false)
			{
				return Unauthorized();
			}

			try
			{
				await currencyService.DeleteCurrency(id, User.Id());

				return NoContent();
			}
			catch (ArgumentNullException ex)
			{
				return NotFound(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}