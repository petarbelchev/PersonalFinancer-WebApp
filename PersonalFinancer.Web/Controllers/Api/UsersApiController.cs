namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.User;
	using Services.User.Models;

	using static Data.Constants.RoleConstants;

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
			UsersServiceModel usersData =
				await usersService.GetAllUsers(page);

			var usersModel = new UsersViewModel
			{
				Users = usersData.Users
			};
			usersModel.Pagination.Page = page;
			usersModel.Pagination.TotalElements = usersData.TotalUsersCount;

			return Ok(usersModel);
		}
	}
}
