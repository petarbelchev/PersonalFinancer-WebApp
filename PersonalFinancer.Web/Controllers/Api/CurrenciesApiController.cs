namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.ApiService;
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
        public async Task<IActionResult> CreateCurrency(CurrencyInputModel inputModel)
        {
            try
            {
                return await this.CreateEntityAsync(inputModel);
            }
            catch (ArgumentException)
            {
                return this.BadRequest($"Currency with the name \"{inputModel.Name}\" exist.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency([Required] Guid? id)
            => await this.DeleteEntityAsync(id);
    }
}