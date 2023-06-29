namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Services.Api;
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
				return this.BadRequest(string.Format(ExceptionMessages.ExistingUserEntityName, "account type", inputModel.Name));
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAccountType([Required] Guid id) 
			=> await this.DeleteEntityAsync(id);
	}
}
