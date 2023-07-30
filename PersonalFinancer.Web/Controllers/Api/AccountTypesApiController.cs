namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Web.Models.Api;
	using System.ComponentModel.DataAnnotations;

	[Route("api/accountTypes")]
	[ApiController]
	public class AccountTypesApiController : BaseApiController<AccountType>
	{
		public AccountTypesApiController(
			IApiService<AccountType> apiService,
			ILogger<BaseApiController<AccountType>> logger)
			: base(apiService, logger)
		{ }

		[HttpPost]
		[Produces("application/json")]
		[ProducesResponseType(typeof(ApiEntityDTO), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateAccountType(AccountTypeInputModel inputModel)
			=> await this.CreateEntityAsync(inputModel);

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> DeleteAccountType([Required] Guid id) 
			=> await this.DeleteEntityAsync(id);
	}
}
