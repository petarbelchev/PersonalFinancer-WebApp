namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Shared;
	using PersonalFinancer.Web.Models.Transaction;
	using static PersonalFinancer.Data.Constants.RoleConstants;

	[Authorize]
	[Route("api/transactions")]
	[ApiController]
	public class TransactionsApiController : ControllerBase
	{
		private readonly IAccountsInfoService accountsInfoService;

		public TransactionsApiController(IAccountsInfoService accountsInfoService) 
			=> this.accountsInfoService = accountsInfoService;

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> GetUserTransactions(UserTransactionsApiInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			Guid userId = this.User.IdToGuid();

			if (inputModel.Id != userId)
				return this.Unauthorized();

			TransactionsServiceModel userTransactions;

			try
			{
				userTransactions = await this.accountsInfoService
					.GetUserTransactionsAsync(userId, inputModel.StartDate, inputModel.EndDate, inputModel.Page);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			var userModel = new TransactionsViewModel
			{
				Transactions = userTransactions.Transactions
			};
			userModel.Pagination.Page = inputModel.Page;
			userModel.Pagination.TotalElements = userTransactions.TotalTransactionsCount;
			userModel.TransactionDetailsUrl = "/Transactions/TransactionDetails/";

			return this.Ok(userModel);
		}
	}
}
