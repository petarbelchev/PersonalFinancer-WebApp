namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Currencies;
	using Services.Currencies.Models;
	using Services.ModelsState;
	using Services.Shared.Models;

	using Web.Infrastructure;

	[Authorize]
	[Route("api/currencies")]
	[ApiController]
	public class CurrenciesApiController : ControllerBase
	{
		private readonly ICurrencyService currencyService;
		private readonly IModelStateService modelStateService;

		public CurrenciesApiController(
			ICurrencyService currencyService,
			IModelStateService modelStateService)
		{
			this.currencyService = currencyService;
			this.modelStateService = modelStateService;
		}

		[HttpPost]
		public async Task<ActionResult<CurrencyServiceModel>> Create(CurrencyInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(modelStateService.GetErrors(ModelState.Values));

			try
			{
				CurrencyServiceModel model =
					await currencyService.CreateCurrency(inputModel);

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
				if (User.IsAdmin())
					await currencyService.DeleteCurrency(id);
				else
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