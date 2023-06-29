namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Services.Api;
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
                return this.BadRequest(string.Format(ExceptionMessages.ExistingUserEntityName, "currency", inputModel.Name));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency([Required] Guid id)
            => await this.DeleteEntityAsync(id);
    }
}