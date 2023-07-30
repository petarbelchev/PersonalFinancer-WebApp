namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Models.Account;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[Authorize]
	[Route("api/accounts")]
	[ApiController]
	public class AccountsApiController : ControllerBase
	{
		private readonly IAccountsInfoService accountsInfoService;
		private readonly IMapper mapper;
		private readonly ILogger<AccountsApiController> logger;

		public AccountsApiController(
			IAccountsInfoService accountsInfoService,
			IMapper mapper,
			ILogger<AccountsApiController> logger)
		{
			this.accountsInfoService = accountsInfoService;
			this.mapper = mapper;
			this.logger = logger;
		}

		[Authorize(Roles = AdminRoleName)]
		[HttpGet("{page}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(UsersAccountsCardsViewModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAccounts(int page)
		{
			AccountsCardsDTO usersCardsData =
				await this.accountsInfoService.GetAccountsCardsDataAsync(page);
			var usersCardsModel = new UsersAccountsCardsViewModel(usersCardsData);

			return this.Ok(usersCardsModel);
		}

		[Authorize(Roles = AdminRoleName)]
		[HttpGet("cashflow")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(IEnumerable<CurrencyCashFlowDTO>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAccountsCashFlow()
			=> this.Ok(await this.accountsInfoService.GetCashFlowByCurrenciesAsync());

		[HttpPost("transactions")]
		[NoHtmlSanitizing]
		[Produces("application/json")]
		[ProducesResponseType(typeof(TransactionsViewModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetAccountTransactions(AccountTransactionsInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.GetAccountTransactionsWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			if (!this.User.IsAdmin() && inputModel.OwnerId != this.User.IdToGuid())
			{
				this.logger.LogWarning(
					LoggerMessages.UnauthorizedGetAccountTransactions,
					this.User.Id(),
					inputModel.Id);

				return this.Unauthorized();
			}

			AccountTransactionsFilterDTO filterDTO = 
				this.mapper.Map<AccountTransactionsFilterDTO>(inputModel);

			TransactionsDTO transactionsDTO =
				await this.accountsInfoService.GetAccountTransactionsAsync(filterDTO);

			var accountTransactions = new TransactionsViewModel(transactionsDTO, inputModel.Page);

			return this.Ok(accountTransactions);
		}
	}
}
