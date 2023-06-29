namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Services.User.Models;
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
        public async Task<ActionResult<UsersViewModel>> AllUsers(int page)
        {
            UsersInfoDTO usersData =
                await this.usersService.GetUsersInfoAsync(page);

            var users = new UsersViewModel(usersData, page);

            return this.Ok(users);
        }
    }
}
