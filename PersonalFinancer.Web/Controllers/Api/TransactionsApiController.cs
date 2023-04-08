﻿namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Accounts;
	using Services.Accounts.Models;

	using Web.Infrastructure;
	using Web.Models.Accounts;
	using Web.Models.Shared;

	using static Data.Constants.RoleConstants;

	[Authorize]
	[Route("api/transactions")]
	[ApiController]
	public class TransactionsApiController : ControllerBase
	{
		private readonly IAccountsService accountService;
		private readonly IMapper mapper;

		public TransactionsApiController(
			IAccountsService accountsService,
			IMapper mapper)
		{
			this.accountService = accountsService;
			this.mapper = mapper;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction(string id)
		{
			try
			{
				decimal newBalance;

				if (User.IsAdmin())
					newBalance = await accountService.DeleteTransaction(id);
				else
					newBalance = await accountService.DeleteTransaction(id, User.Id());

				return Ok(new { newBalance });
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> GetUserTransactions(UserTransactionsApiInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			if (inputModel.Id != User.Id())
				return Unauthorized();

			try
			{
				var inputDTO = mapper.Map<UserTransactionsApiInputDTO>(inputModel);
				var viewModel = new TransactionsViewModel();
				inputDTO.ElementsPerPage = viewModel.Pagination.ElementsPerPage;

				UserTransactionsApiOutputDTO transactionsData =
					await accountService.GetUserTransactionsApi(inputDTO);

				viewModel.Transactions = transactionsData.Transactions
					.Select(t => mapper.Map<TransactionTableViewModel>(t));
				viewModel.Pagination.Page = transactionsData.Page;
				viewModel.Pagination.TotalElements = transactionsData.AllTransactionsCount;
				viewModel.TransactionDetailsUrl = "/Transactions/TransactionDetails/";

				return Ok(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}