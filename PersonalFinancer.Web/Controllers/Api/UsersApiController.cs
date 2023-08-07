namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Models.Api;
	using PersonalFinancer.Web.Models.User;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[Authorize(Roles = AdminRoleName)]
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly IUsersService usersService;
		private readonly ILogger<UsersApiController> logger;

        public UsersApiController(
			IUsersService usersService, 
			ILogger<UsersApiController> logger)
		{
			this.usersService = usersService;
			this.logger = logger;
		}

		[HttpPost]
		[NoHtmlSanitizing]
		[Produces("application/json")]
		[ProducesResponseType(typeof(UsersViewModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Get(SearchFilterInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.GetUsersInfoWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			UsersInfoDTO usersData =
				await this.usersService.GetUsersInfoAsync(inputModel.Page, inputModel.Search);

			var users = new UsersViewModel(usersData, inputModel.Page);

			return this.Ok(users);
		}
	}
}
