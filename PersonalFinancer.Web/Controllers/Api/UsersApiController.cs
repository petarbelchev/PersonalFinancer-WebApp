using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize(Roles = AdminRoleName)]
	[Route("api/users")]
	[ApiController]
	public class UsersApiController : ControllerBase
	{
		private readonly IUsersService usersService;

		public UsersApiController(IUsersService usersService)
			=> this.usersService = usersService;

		[HttpGet("{page}")]
		public async Task<ActionResult<AllUsersViewModel>> AllUsers(int page)
			=> Ok(await usersService.GetAllUsers(page));
	}
}
