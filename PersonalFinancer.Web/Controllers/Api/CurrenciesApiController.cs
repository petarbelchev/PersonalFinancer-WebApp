namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Services.ApiService;
    using PersonalFinancer.Services.ApiService.Models;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using PersonalFinancer.Web.Models.Api;
    using System.ComponentModel.DataAnnotations;

    [Route("api/currencies")]
    [ApiController]
    public class CurrenciesApiController : BaseApiController
    {
        private readonly IApiService<Currency> apiService;

        public CurrenciesApiController(IApiService<Currency> apiService)
            => this.apiService = apiService;

        [HttpPost]
        public async Task<ActionResult> Create(CurrencyInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
                return this.BadRequest(this.GetErrors(this.ModelState.Values));

            try
            {
                ApiOutputServiceModel model = await this.apiService.CreateEntity(
                    inputModel.Name,
                    inputModel.OwnerId ?? throw new InvalidOperationException());

                return this.Created(string.Empty, model);
            }
            catch (ArgumentException ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([Required] Guid? id)
        {
            if (!this.ModelState.IsValid)
                return this.BadRequest(this.GetErrors(this.ModelState.Values));

            try
            {
                await this.apiService.DeleteEntity(
                    id ?? throw new InvalidOperationException(),
                    this.User.IdToGuid(), this.User.IsAdmin());

                return this.NoContent();
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }
    }
}