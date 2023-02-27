namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;

	using Services.Transactions;

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
			bool isDeleted = await transactionsService.DeleteTransactionById(id);

			if (!isDeleted)
			{
				return BadRequest();
			}

			return NoContent();
		}
	}
}
