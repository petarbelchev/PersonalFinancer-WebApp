using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Web.Infrastructure;
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
			=> Ok(await accountService.GetUsersAccountCardsViewModel(page));

		[Authorize(Roles = RoleConstants.AdminRoleName)]
		[HttpGet("cashflow")]
		public async Task<IActionResult> GetAccountsCashFlow()
			=> Ok(await accountService.GetUsersCurrenciesCashFlow());

		[HttpPost("transactions")]
		public async Task<IActionResult> GetAccountTransactions(
			AccountTransactionsInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			if (!User.IsAdmin() && inputModel.OwnerId != User.Id())
				return Unauthorized();

			try
			{
				TransactionsViewModel viewModel = 
					await accountService.GetAccountTransactions(inputModel);

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
