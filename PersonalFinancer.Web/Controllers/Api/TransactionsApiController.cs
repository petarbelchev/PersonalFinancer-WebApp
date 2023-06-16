namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Transactions;
    using PersonalFinancer.Services.Transactions.Models;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using PersonalFinancer.Web.Models.Shared;
    using PersonalFinancer.Web.Models.Transaction;
    using System.Globalization;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    [Authorize]
	[Route("api/transactions")]
	[ApiController]
	public class TransactionsApiController : ControllerBase
	{
		private readonly ITransactionsInfoService transactionsInfoService;

		public TransactionsApiController(ITransactionsInfoService transactionsInfoService) 
			=> this.transactionsInfoService = transactionsInfoService;

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> GetUserTransactions(UserTransactionsApiInputModel inputModel)
		{
			bool isStartDateValid = DateTime.TryParse(
				inputModel.StartDate, null, DateTimeStyles.None, out DateTime startDate);

			bool isEndDateValid = DateTime.TryParse(
				inputModel.EndDate, null, DateTimeStyles.None, out DateTime endDate);

			if (!this.ModelState.IsValid || !isStartDateValid || !isEndDateValid || startDate > endDate)
				return this.BadRequest();

			Guid userId = this.User.IdToGuid();

			if (inputModel.Id != userId)
				return this.Unauthorized();

			TransactionsServiceModel userTransactions;

			try
			{
				userTransactions = await this.transactionsInfoService
					.GetUserTransactionsAsync(userId, startDate, endDate, inputModel.Page);
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
