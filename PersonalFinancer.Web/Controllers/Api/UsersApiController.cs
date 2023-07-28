namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.Models.User;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[Authorize(Roles = AdminRoleName)]
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly IUsersService usersService;

        public UsersApiController(IUsersService usersService)
            => this.usersService = usersService;

        [HttpGet("{page}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UsersViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> AllUsers(int page)
        {
            UsersInfoDTO usersData =
                await this.usersService.GetUsersInfoAsync(page);

            var users = new UsersViewModel(usersData, page);

            return this.Ok(users);
        }
    }
}
