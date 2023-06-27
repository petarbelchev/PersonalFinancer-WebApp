﻿namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Account;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Data.Constants;
	using static PersonalFinancer.Web.Constants;

	[Authorize]
	[Route("api/accounts")]
	[ApiController]
	public class AccountsApiController : ControllerBase
	{
		private readonly IAccountsInfoService accountsInfoService;
		private readonly IMapper mapper;

		public AccountsApiController(
			IAccountsInfoService accountsInfoService,
			IMapper mapper)
		{
			this.accountsInfoService = accountsInfoService;
			this.mapper = mapper;
		}

		[Authorize(Roles = RoleConstants.AdminRoleName)]
		[HttpGet("{page}")]
		public async Task<IActionResult> GetAccounts(int page)
		{
			AccountsCardsDTO usersCardsData =
				await this.accountsInfoService.GetAccountsCardsDataAsync(page);
			var usersCardsModel = new UsersAccountsCardsViewModel(usersCardsData);

			return this.Ok(usersCardsModel);
		}

		[Authorize(Roles = RoleConstants.AdminRoleName)]
		[HttpGet("cashflow")]
		public async Task<IActionResult> GetAccountsCashFlow()
			=> this.Ok(await this.accountsInfoService.GetCashFlowByCurrenciesAsync());

		[HttpPost("transactions")]
		public async Task<IActionResult> GetAccountTransactions(AccountTransactionsInputModel inputModel)
		{
			// TODO: Try to use default api validation.

			if (!this.ModelState.IsValid)
				return this.BadRequest();

			if (!this.User.IsAdmin() && inputModel.OwnerId != this.User.IdToGuid())
				return this.Unauthorized();

			var filterDTO = this.mapper.Map<AccountTransactionsFilterDTO>(inputModel);

			TransactionsDTO transactionsDTO =
				await this.accountsInfoService.GetAccountTransactionsAsync(filterDTO);

			string transactionDetailsUrlPath = this.User.IsAdmin()
				? UrlPathConstants.AdminTransactionDetailsPath
				: UrlPathConstants.TransactionDetailsPath;

			var accountTransactions = new TransactionsViewModel(
				transactionsDTO, inputModel.Page, transactionDetailsUrlPath);

			return this.Ok(accountTransactions);
		}
	}
}
