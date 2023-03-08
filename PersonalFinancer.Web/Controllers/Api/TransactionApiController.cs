using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Transactions;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Route("api/transactions")]
	[ApiController]
	public class TransactionApiController : ControllerBase
	{
		private readonly ITransactionsService transactionsService;

		public TransactionApiController(ITransactionsService transactionsService)
		{
			this.transactionsService = transactionsService;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction(Guid id)
		{
			if (!User.Identity?.IsAuthenticated ?? false)
			{
				return Unauthorized();
			}

			try
			{
				decimal newBalance = await transactionsService.DeleteTransactionById(id);
				return Ok(new { newBalance });
			}
			catch (ArgumentNullException)
			{
				return NotFound();
			}
		}
	}
}
