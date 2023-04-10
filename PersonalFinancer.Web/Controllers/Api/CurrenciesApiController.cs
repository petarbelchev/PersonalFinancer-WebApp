namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Currencies;
	using Services.Currencies.Models;

	using Web.Infrastructure;
	using Web.Models.Currencies;
	using Web.Models.Shared;

	[Authorize]
	[Route("api/currencies")]
	[ApiController]
	public class CurrenciesApiController : ControllerBase
	{
		private readonly ICurrencyService currencyService;
		private readonly IControllerService controllerService;
		private readonly IMapper mapper;

		public CurrenciesApiController(
			ICurrencyService currencyService,
			IControllerService controllerService,
			IMapper mapper)
		{
			this.currencyService = currencyService;
			this.controllerService = controllerService;
			this.mapper = mapper;
		}

		[HttpPost]
		public async Task<ActionResult<CurrencyViewModel>> Create(CurrencyInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(controllerService.GetModelErrors(ModelState.Values));

			try
			{
				var inputDTO = mapper.Map<CurrencyInputDTO>(inputModel);

				CurrencyOutputDTO currencyData =
					await currencyService.CreateCurrency(inputDTO);

				var viewModel = mapper.Map<CurrencyViewModel>(currencyData);

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