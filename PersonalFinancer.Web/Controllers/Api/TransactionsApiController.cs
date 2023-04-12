namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using System.Globalization;
	
	using Services.Accounts;
	using Services.Shared.Models;
	using Services.User;
	
	using Web.Infrastructure;
	using Web.Models.Shared;
	using Web.Models.Transaction;
	
	using static Data.Constants.RoleConstants;

	[Authorize]
	[Route("api/transactions")]
	[ApiController]
	public class TransactionsApiController : ControllerBase
	{
		private readonly IAccountsService accountService;
		private readonly IUsersService usersService;

		public TransactionsApiController(
			IAccountsService accountsService,
			IUsersService usersService)
		{
			this.accountService = accountsService;
			this.usersService = usersService;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction(string id)
		{
			try
			{
				decimal newBalance;

				if (User.IsAdmin())
					newBalance = await accountService.DeleteTransaction(id);
				else
					newBalance = await accountService.DeleteTransaction(id, User.Id());

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

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> GetUserTransactions(UserTransactionsApiInputModel inputModel)
		{
			bool isStartDateValid = DateTime.TryParse(
				inputModel.StartDate, null, DateTimeStyles.None, out DateTime startDate);

			bool isEndDateValid = DateTime.TryParse(
				inputModel.EndDate, null, DateTimeStyles.None, out DateTime endDate);

			if (!ModelState.IsValid || !isStartDateValid || !isEndDateValid || startDate > endDate)
				return BadRequest();

			if (inputModel.Id != User.Id())
				return Unauthorized();

			try
			{
				TransactionsServiceModel userTransactions = await usersService
					.GetUserTransactions(inputModel.Id, startDate, endDate, inputModel.Page);

				var userModel = new TransactionsViewModel
				{
					Transactions = userTransactions.Transactions
				};
				userModel.Pagination.Page = inputModel.Page;
				userModel.Pagination.TotalElements = userTransactions.TotalTransactionsCount;
				userModel.TransactionDetailsUrl = "/Transactions/TransactionDetails/";

				return Ok(userModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
