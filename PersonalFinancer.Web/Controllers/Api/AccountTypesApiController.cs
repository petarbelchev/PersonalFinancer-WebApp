namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Services.ApiService;
    using PersonalFinancer.Services.ApiService.Models;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using PersonalFinancer.Web.Models.Api;
    using System.ComponentModel.DataAnnotations;

    [Route("api/accounttypes")]
    [ApiController]
    public class AccountTypesApiController : BaseApiController
    {
        private readonly IApiService<AccountType> apiService;

        public AccountTypesApiController(IApiService<AccountType> apiService)
            => this.apiService = apiService;

        [HttpPost]
        public async Task<ActionResult> Create(AccountTypeInputModel inputModel)
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
            catch (ArgumentException)
            {
                return this.BadRequest($"Account Type with the name \"{inputModel.Name}\" exist.");
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
                    this.User.Id(), this.User.IsAdmin());

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
