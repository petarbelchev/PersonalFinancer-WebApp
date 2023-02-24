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

		[HttpGet("{id}")]
		[Route("delete")]
		public async Task<IActionResult> DeleteTransaction(Guid transactionId)
		{
			bool isDeleted = await accountService.DeleteTransactionById(transactionId);

			if (!isDeleted)
			{
				return NotFound();
			}

			return Ok(transactionId);
		}
	}
}
