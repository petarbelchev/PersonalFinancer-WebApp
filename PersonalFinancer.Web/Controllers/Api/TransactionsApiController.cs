namespace PersonalFinancer.Web.Controllers.Api
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Api;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Data.Constants.RoleConstants;
	using static PersonalFinancer.Web.Constants;

	[Authorize]
	[Route("api/transactions")]
	[ApiController]
	public class TransactionsApiController : ControllerBase
	{
		private readonly IUsersService usersService;
		private readonly IMapper mapper;

		public TransactionsApiController(
			IUsersService usersService,
			IMapper mapper)
		{
			this.usersService = usersService;
			this.mapper = mapper;
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> GetUserTransactions(UserTransactionsApiInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			Guid userId = this.User.IdToGuid();

			if (inputModel.Id != userId)
				return this.Unauthorized();

			var filterDTO = this.mapper.Map<TransactionsFilterDTO>(inputModel);
			TransactionsDTO transactionsDTO;

			try
			{
				transactionsDTO = await this.usersService.GetUserTransactionsAsync(filterDTO);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			var userTransactions = new TransactionsViewModel(
				transactionsDTO,
				inputModel.Page,
				UrlPathConstants.TransactionDetailsPath);

			return this.Ok(userTransactions);
		}
	}
}
