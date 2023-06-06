using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Models.Account;
using PersonalFinancer.Web.Models.Shared;
using System.Globalization;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize]
	[Route("api/accounts")]
	[ApiController]
	public class AccountsApiController : ControllerBase
	{
		private IAccountsService accountService;

		public AccountsApiController(IAccountsService accountService)
		{
			this.accountService = accountService;
		}

		[Authorize(Roles = RoleConstants.AdminRoleName)]
		[HttpGet("{page}")]
		public async Task<IActionResult> GetAccounts(int page)
		{
			UsersAccountCardsServiceModel usersCardsData =
				await accountService.GetAccountCardsData(page);

			var usersCardsModel = new UsersAccountCardsViewModel
			{
				Accounts = usersCardsData.Accounts
			};
			usersCardsModel.Pagination.TotalElements = usersCardsData.TotalUsersAccountsCount;

			return Ok(usersCardsModel);
		}

		[Authorize(Roles = RoleConstants.AdminRoleName)]
		[HttpGet("cashflow")]
		public async Task<IActionResult> GetAccountsCashFlow()
			=> Ok(await accountService.GetCurrenciesCashFlow());

		[HttpPost("transactions")]
		public async Task<IActionResult> GetAccountTransactions(AccountTransactionsInputModel inputModel)
		{
			bool isStartDateValid = DateTime.TryParse(
				inputModel.StartDate, null, DateTimeStyles.None, out DateTime startDate);

			bool isEndDateValid = DateTime.TryParse(
				inputModel.EndDate, null, DateTimeStyles.None, out DateTime endDate);

			if (!ModelState.IsValid || !isStartDateValid || !isEndDateValid || startDate > endDate)
				return BadRequest();

			if (!User.IsAdmin() && inputModel.OwnerId != User.Id())
				return Unauthorized();

			try
			{
				TransactionsServiceModel accountTransactions = await accountService
					.GetAccountTransactions(inputModel.Id, startDate, endDate, inputModel.Page);

				var viewModel = new TransactionsViewModel
				{
					Transactions = accountTransactions.Transactions
				};
				viewModel.Pagination.TotalElements = accountTransactions.TotalTransactionsCount;
				viewModel.Pagination.Page = inputModel.Page;
				viewModel.TransactionDetailsUrl =
					$"{(User.IsAdmin() ? "/Admin" : string.Empty)}/Transactions/TransactionDetails/";

				return Ok(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
