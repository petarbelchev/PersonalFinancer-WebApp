namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.ApiService;
	using PersonalFinancer.Web.Models.Api;
	using System.ComponentModel.DataAnnotations;

	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypesApiController : BaseApiController<AccountType>
	{
		public AccountTypesApiController(IApiService<AccountType> apiService)
			: base(apiService)
		{ }

		[HttpPost]
		public async Task<IActionResult> CreateAccountType(AccountTypeInputModel inputModel)
		{
			try
			{
				return await this.CreateEntityAsync(inputModel);
			}
			catch (ArgumentException)
			{
				return this.BadRequest($"Account Type with the name \"{inputModel.Name}\" exist.");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAccountType([Required] Guid? id) 
			=> await this.DeleteEntityAsync(id);
	}
}
