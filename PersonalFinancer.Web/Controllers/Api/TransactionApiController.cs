namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;

	using Services.Account;

	[Route("api/transactions")]
	[ApiController]
	public class TransactionApiController : ControllerBase
	{
		private readonly IAccountService accountService;

		public TransactionApiController(IAccountService accountService)
		{
			this.accountService = accountService;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction(Guid id)
		{
			bool isDeleted = await accountService.DeleteTransactionById(id);

			if (!isDeleted)
			{
				return BadRequest();
			}

			return NoContent();
		}
	}
}
