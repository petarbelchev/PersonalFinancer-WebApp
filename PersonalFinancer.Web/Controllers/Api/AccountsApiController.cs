namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Extensions;
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

		public AccountsApiController(
			IAccountsInfoService accountsInfoService,
			IMapper mapper)
		{
			this.accountsInfoService = accountsInfoService;
			this.mapper = mapper;
		}

		[Authorize(Roles = AdminRoleName)]
		[HttpGet("{page}")]
		public async Task<IActionResult> GetAccounts(int page)
		{
			AccountsCardsDTO usersCardsData =
				await this.accountsInfoService.GetAccountsCardsDataAsync(page);
			var usersCardsModel = new UsersAccountsCardsViewModel(usersCardsData);

			return this.Ok(usersCardsModel);
		}

		[Authorize(Roles = AdminRoleName)]
		[HttpGet("cashflow")]
		public async Task<IActionResult> GetAccountsCashFlow()
			=> this.Ok(await this.accountsInfoService.GetCashFlowByCurrenciesAsync());

		[HttpPost("transactions")]
		[NotRequireHtmlEncoding]
		public async Task<IActionResult> GetAccountTransactions(AccountTransactionsInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			if (!this.User.IsAdmin() && inputModel.OwnerId != this.User.IdToGuid())
				return this.Unauthorized();

			AccountTransactionsFilterDTO filterDTO = 
				this.mapper.Map<AccountTransactionsFilterDTO>(inputModel);

			TransactionsDTO transactionsDTO =
				await this.accountsInfoService.GetAccountTransactionsAsync(filterDTO);

			var accountTransactions = new TransactionsViewModel(transactionsDTO, inputModel.Page);

			return this.Ok(accountTransactions);
		}
	}
}
