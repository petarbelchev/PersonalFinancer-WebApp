namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Web.Models.Api;
	using System.ComponentModel.DataAnnotations;

	[Route("api/currencies")]
    [ApiController]
    public class CurrenciesApiController : BaseApiController<Currency>
    {
        public CurrenciesApiController(
			IApiService<Currency> apiService,
			ILogger<BaseApiController<Currency>> logger)
            : base(apiService, logger)
        { }

		[HttpPost]
		[Produces("application/json")]
		[ProducesResponseType(typeof(ApiEntityDTO), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateCurrency(CurrencyInputModel inputModel)
			=> await this.CreateEntityAsync(inputModel);

        [HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> DeleteCurrency([Required] Guid id)
            => await this.DeleteEntityAsync(id);
    }
}