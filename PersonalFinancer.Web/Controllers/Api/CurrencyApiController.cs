namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;

	using PersonalFinancer.Services.Currency;
	using PersonalFinancer.Web.Infrastructure;
	using PersonalFinancer.Services.Currency.Models;
	using static PersonalFinancer.Data.Constants.CurrencyConstants;

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

			if (model.Name.Length < CurrencyNameMinLength || model.Name.Length > CurrencyNameMaxLength)
			{
				return BadRequest($"Currency name must be between {CurrencyNameMinLength} and {CurrencyNameMaxLength} characters long.");
			}

			try
			{
				CurrencyViewModel currency = await currencyService.CreateCurrency(User.Id(), model.Name);

				return Created(string.Empty, currency);
			}
			catch (InvalidOperationException)
			{
				return BadRequest("Currency with the same name exist. Try another one!");
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