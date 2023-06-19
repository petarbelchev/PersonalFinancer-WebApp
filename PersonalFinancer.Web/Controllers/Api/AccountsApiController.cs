﻿namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Account;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Data.Constants;

	[Authorize]
	[Route("api/accounts")]
	[ApiController]
	public class AccountsApiController : ControllerBase
	{
		private readonly IAccountsInfoService accountsInfoService;

		public AccountsApiController(IAccountsInfoService accountsInfoService)
			=> this.accountsInfoService = accountsInfoService;

		[Authorize(Roles = RoleConstants.AdminRoleName)]
		[HttpGet("{page}")]
		public async Task<IActionResult> GetAccounts(int page)
		{
			UsersAccountsCardsServiceModel usersCardsData =
				await this.accountsInfoService.GetAccountsCardsDataAsync(page);

			var usersCardsModel = new UsersAccountCardsViewModel
			{
				Accounts = usersCardsData.Accounts
			};

			usersCardsModel.Pagination.TotalElements = usersCardsData.TotalUsersAccountsCount;

			return this.Ok(usersCardsModel);
		}

		[Authorize(Roles = RoleConstants.AdminRoleName)]
		[HttpGet("cashflow")]
		public async Task<IActionResult> GetAccountsCashFlow()
			=> this.Ok(await this.accountsInfoService.GetCashFlowByCurrenciesAsync());

		[HttpPost("transactions")]
		public async Task<IActionResult> GetAccountTransactions(AccountTransactionsInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			if (!this.User.IsAdmin() && inputModel.OwnerId != this.User.IdToGuid())
				return this.Unauthorized();

			TransactionsServiceModel accountTransactions =
				await this.accountsInfoService.GetAccountTransactionsAsync(
					inputModel.Id ?? throw new InvalidOperationException(),
					inputModel.StartDate, inputModel.EndDate, inputModel.Page);

			var viewModel = new TransactionsViewModel
			{
				Transactions = accountTransactions.Transactions
			};

			viewModel.Pagination.TotalElements = accountTransactions.TotalTransactionsCount;
			viewModel.Pagination.Page = inputModel.Page;
			viewModel.TransactionDetailsUrl =
				$"{(this.User.IsAdmin() ? "/Admin" : string.Empty)}/Transactions/TransactionDetails/";

			return this.Ok(viewModel);
		}
	}
}
