using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize]
	[Route("api/transactions")]
	[ApiController]
	public class TransactionsApiController : ControllerBase
	{
		private readonly IAccountsService accountsService;

		public TransactionsApiController(IAccountsService accountsService)
		{
			this.accountsService = accountsService;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction(string id)
		{
			try
			{
				decimal newBalance;

				if (User.IsAdmin())
					newBalance = await accountsService.DeleteTransaction(id);
				else
					newBalance = await accountsService.DeleteTransaction(id, User.Id());

				return Ok(new { newBalance });
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
