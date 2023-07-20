namespace PersonalFinancer.Web.Controllers.Api
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Extensions;
    using PersonalFinancer.Web.Models.Api;
    using PersonalFinancer.Web.Models.Shared;
    using static PersonalFinancer.Common.Constants.RoleConstants;

    [Authorize]
	[Route("api/transactions")]
	[ApiController]
	public class TransactionsApiController : ControllerBase
	{
		private readonly IUsersService usersService;
		private readonly IAccountsInfoService accountsInfoService;
		private readonly IAccountsUpdateService accountsUpdateService;
		private readonly IMapper mapper;

		public TransactionsApiController(
			IUsersService usersService,
			IAccountsInfoService accountsInfoService,
			IAccountsUpdateService accountsUpdateService,
			IMapper mapper)
		{
			this.usersService = usersService;
			this.accountsInfoService = accountsInfoService;
			this.accountsUpdateService = accountsUpdateService;
			this.mapper = mapper;
		}

		[HttpDelete("{id}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> DeleteTransaction(Guid id)
		{
			decimal balance;

			try
			{
				balance = await this.accountsUpdateService
					.DeleteTransactionAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (ArgumentException)
			{
				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			string message = this.User.IsAdmin()
				? ResponseMessages.AdminDeletedUserTransaction
				: ResponseMessages.DeletedTransaction;

			return this.Ok(new { message, balance });
		}

		[HttpGet("{id}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(TransactionDetailsDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetTransactionDetails(Guid id)
		{
			TransactionDetailsDTO model;

			try
			{
				model = await this.accountsInfoService
					.GetTransactionDetailsAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (ArgumentException)
			{
				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.Ok(model);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		[Produces("application/json")]
		[ProducesResponseType(typeof(TransactionsViewModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[NoHtmlSanitizing]
		public async Task<IActionResult> GetUserTransactions(UserTransactionsApiInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			if (inputModel.Id != this.User.IdToGuid())
				return this.Unauthorized();

			TransactionsFilterDTO filterDTO = this.mapper.Map<TransactionsFilterDTO>(inputModel);
			TransactionsDTO transactionsDTO;

			try
			{
				transactionsDTO = await this.usersService.GetUserTransactionsAsync(filterDTO);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			var userTransactions = new TransactionsViewModel(transactionsDTO, inputModel.Page);

			return this.Ok(userTransactions);
		}
	}
}
