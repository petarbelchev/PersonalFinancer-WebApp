﻿namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.AccountTypes;
	using Services.AccountTypes.Models;

	using Web.Infrastructure;
	using Web.Models.AccountTypes;
	using Web.Models.Shared;

	[Authorize]
	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypesApiController : ControllerBase
	{
		private readonly IAccountTypeService accountTypeService;
		private readonly IControllerService controllerService;
		private readonly IMapper mapper;

		public AccountTypesApiController(
			IAccountTypeService accountTypeService,
			IControllerService controllerService,
			IMapper mapper)
		{
			this.accountTypeService = accountTypeService;
			this.controllerService = controllerService;
			this.mapper = mapper;
		}

		[HttpPost]
		public async Task<ActionResult<AccountTypeViewModel>> Create(AccountTypeInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(controllerService.GetModelErrors(ModelState.Values));

			try
			{
				var accountTypeInputDTO =
					mapper.Map<AccountTypeInputDTO>(inputModel);

				var accountTypeOutputDTO =
					await accountTypeService.CreateAccountType(accountTypeInputDTO);

				var viewModel = mapper.Map<AccountTypeViewModel>(accountTypeOutputDTO);

				return Created(string.Empty, viewModel);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(string id)
		{
			try
			{
				if (User.IsAdmin())
					await accountTypeService.DeleteAccountType(id);
				else
					await accountTypeService.DeleteAccountType(id, User.Id());

				return NoContent();
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
	}
}