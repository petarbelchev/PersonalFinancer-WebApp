﻿namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Models.Api;
	using PersonalFinancer.Web.Models.Shared;
	using System.ComponentModel.DataAnnotations;
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
		private readonly ILogger<TransactionsApiController> logger;

		public TransactionsApiController(
			IUsersService usersService,
			IAccountsInfoService accountsInfoService,
			IAccountsUpdateService accountsUpdateService,
			IMapper mapper,
			ILogger<TransactionsApiController> logger)
		{
			this.usersService = usersService;
			this.accountsInfoService = accountsInfoService;
			this.accountsUpdateService = accountsUpdateService;
			this.mapper = mapper;
			this.logger = logger;
		}

		[HttpDelete("{id}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(DeleteTransactionOutputModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> DeleteTransaction([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.DeleteTransactionWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			var outputModel = new DeleteTransactionOutputModel();

			try
			{
				outputModel.Balance = await this.accountsUpdateService
					.DeleteTransactionAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (UnauthorizedAccessException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnauthorizedTransactionDeletion,
					this.User.Id(),
					id);

				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				this.logger.LogWarning(
					LoggerMessages.DeleteTransactionWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			outputModel.Message = this.User.IsAdmin()
				? ResponseMessages.AdminDeletedUserTransaction
				: ResponseMessages.DeletedTransaction;

			return this.Ok(outputModel);
		}

		[HttpGet("{id}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(TransactionDetailsDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetTransactionDetails([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.GetTransactionDetailsWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			TransactionDetailsDTO model;

			try
			{
				model = await this.accountsInfoService
					.GetTransactionDetailsAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (UnauthorizedAccessException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnauthorizedGetTransactionDetails,
					this.User.Id(),
					id);

				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				this.logger.LogWarning(
					LoggerMessages.GetTransactionDetailsWithInvalidInputData,
					this.User.Id());

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
			{
				this.logger.LogWarning(
					LoggerMessages.GetUserTransactionsWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			if (inputModel.Id != this.User.IdToGuid())
			{
				this.logger.LogWarning(
					LoggerMessages.UnauthorizedGetUserTransactions,
					this.User.Id(),
					inputModel.Id);

				return this.Unauthorized();
			}

			var filterDTO = this.mapper.Map<TransactionsFilterDTO>(inputModel);
			var transactionsDTO = await this.usersService.GetUserTransactionsAsync(filterDTO);
			var userTransactions = new TransactionsViewModel(transactionsDTO, inputModel.Page);

			return this.Ok(userTransactions);
		}
	}
}
