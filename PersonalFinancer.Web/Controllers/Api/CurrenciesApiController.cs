namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Web.Models.Api;
    using System.ComponentModel.DataAnnotations;

    [Route("api/currencies")]
    [ApiController]
    public class CurrenciesApiController : BaseApiController<Currency>
    {
        public CurrenciesApiController(IApiService<Currency> apiService)
            : base(apiService)
        { }

        [HttpPost]
		[Produces("application/json")]
		[ProducesResponseType(typeof(ApiEntityDTO), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateCurrency(CurrencyInputModel inputModel)
        {
            try
            {
                return await this.CreateEntityAsync(inputModel);
            }
            catch (ArgumentException)
            {
                return this.BadRequest(string.Format(ExceptionMessages.ExistingUserEntityName, "currency", inputModel.Name));
            }
        }

        [HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> DeleteCurrency([Required] Guid id)
            => await this.DeleteEntityAsync(id);
    }
}