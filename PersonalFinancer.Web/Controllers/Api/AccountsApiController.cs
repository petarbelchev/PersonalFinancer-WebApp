namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Accounts;
	using Services.Accounts.Models;

	using Web.Infrastructure;
	using Web.Models.Accounts;
	using Web.Models.Shared;

	using static Data.Constants;

	[Authorize]
	[Route("api/accounts")]
	[ApiController]
	public class AccountsApiController : ControllerBase
	{
		private IAccountsService accountService;
		private IMapper mapper;

		public AccountsApiController(
			IAccountsService accountService,
			IMapper mapper)
		{
			this.accountService = accountService;
			this.mapper = mapper;
		}

		[Authorize(Roles = RoleConstants.AdminRoleName)]
		[HttpGet("{page}")]
		public async Task<IActionResult> GetAccounts(int page)
		{
			var viewModel = new UsersAccountCardsViewModel();

			AccountCardsOutputDTO accountCardsData = await accountService
				.GetUsersAccountCards(page, viewModel.Pagination.ElementsPerPage);

			viewModel.Accounts = accountCardsData.Accounts
				.Select(a => mapper.Map<AccountCardExtendedViewModel>(a));
			viewModel.Pagination.Page = accountCardsData.Page;
			viewModel.Pagination.TotalElements = accountCardsData.AllAccountsCount;

			return Ok(viewModel);
		}

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
				var inputDTO = mapper.Map<AccountTransactionsInputDTO>(inputModel);
				var viewModel = new TransactionsViewModel();
				inputDTO.ElementsPerPage = viewModel.Pagination.ElementsPerPage;

				AccountTransactionsOutputDTO transactionsData =
					await accountService.GetAccountTransactions(inputDTO);

				viewModel.Transactions = transactionsData.Transactions
					.Select(t => mapper.Map<TransactionTableViewModel>(t));
				viewModel.Pagination.Page = transactionsData.Page;
				viewModel.Pagination.TotalElements = transactionsData.AllTransactionsCount;
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
