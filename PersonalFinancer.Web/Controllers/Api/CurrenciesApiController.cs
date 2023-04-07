using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Currencies;
using PersonalFinancer.Services.Currencies.Models;
using PersonalFinancer.Services.ModelsState;
using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Models.Currencies;
using PersonalFinancer.Web.Models.Shared;

namespace PersonalFinancer.Web.Controllers.Api
{
    [Authorize]
	[Route("api/currencies")]
	[ApiController]
	public class CurrenciesApiController : ControllerBase
	{
		private readonly ICurrencyService currencyService;
		private readonly IModelStateService modelStateService;
		private readonly IMapper mapper;

		public CurrenciesApiController(
			ICurrencyService currencyService,
			IModelStateService modelStateService,
			IMapper mapper)
		{
			this.currencyService = currencyService;
			this.modelStateService = modelStateService;
			this.mapper = mapper;
		}

		[HttpPost]
		public async Task<ActionResult<CurrencyViewModel>> Create(CurrencyInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(modelStateService.GetErrors(ModelState.Values));

			try
			{
				// NOTE: Think to simplify models... 
				var inputData = mapper.Map<CurrencyInputDTO>(inputModel);

				CurrencyOutputDTO currencyData = 
					await currencyService.CreateCurrency(inputData);

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