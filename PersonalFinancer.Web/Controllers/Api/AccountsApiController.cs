namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Transactions;
    using PersonalFinancer.Services.Transactions.Models;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using PersonalFinancer.Web.Models.Account;
    using PersonalFinancer.Web.Models.Shared;
    using System.Globalization;
    using static PersonalFinancer.Data.Constants;

    [Authorize]
	[Route("api/accounts")]
	[ApiController]
	public class AccountsApiController : ControllerBase
	{
		private readonly IAccountsService accountsService;
		private readonly ITransactionsInfoService transactionsInfoService;

		public AccountsApiController(
			IAccountsService accountService,
			ITransactionsInfoService transactionsInfoService)
		{
			this.accountsService = accountService;
			this.transactionsInfoService = transactionsInfoService;
		}

		[Authorize(Roles = RoleConstants.AdminRoleName)]
		[HttpGet("{page}")]
		public async Task<IActionResult> GetAccounts(int page)
		{
			UsersAccountsCardsServiceModel usersCardsData =
				await this.accountsService.GetAccountsCardsDataAsync(page);

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
			=> this.Ok(await this.accountsService.GetCashFlowByCurrenciesAsync());

		[HttpPost("transactions")]
		public async Task<IActionResult> GetAccountTransactions(AccountTransactionsInputModel inputModel)
		{
			// TODO: Try to use input model with DateTime props

			bool isStartDateValid = DateTime.TryParse(
				inputModel.StartDate, null, DateTimeStyles.None, out DateTime startDate);

			bool isEndDateValid = DateTime.TryParse(
				inputModel.EndDate, null, DateTimeStyles.None, out DateTime endDate);

			if (!this.ModelState.IsValid || !isStartDateValid || !isEndDateValid || startDate > endDate)
				return this.BadRequest();

			if (!this.User.IsAdmin() && inputModel.OwnerId != this.User.IdToGuid())
				return this.Unauthorized();

			TransactionsServiceModel accountTransactions = 
				await this.transactionsInfoService.GetAccountTransactionsAsync(
					inputModel.Id ?? throw new InvalidOperationException(), 
					startDate, endDate, inputModel.Page);

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
