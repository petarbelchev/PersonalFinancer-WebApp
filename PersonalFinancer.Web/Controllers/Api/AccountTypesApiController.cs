using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.ModelsState;
using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Models.AccountTypes;
using PersonalFinancer.Web.Models.Shared;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize]
	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypesApiController : ControllerBase
	{
		private readonly IAccountTypeService accountTypeService;
		private readonly IModelStateService modelStateService;
		private readonly IMapper mapper;

		public AccountTypesApiController(
			IAccountTypeService accountTypeService,
			IModelStateService modelStateService,
			IMapper mapper)
		{
			this.accountTypeService = accountTypeService;
			this.modelStateService = modelStateService;
			this.mapper = mapper;
		}

		[HttpPost]
		public async Task<ActionResult<AccountTypeViewModel>> Create(AccountTypeInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(modelStateService.GetErrors(ModelState.Values));

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
