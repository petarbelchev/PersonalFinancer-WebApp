using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Models.User;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize(Roles = AdminRoleName)]
	[Route("api/users")]
	[ApiController]
	public class UsersApiController : ControllerBase
	{
		private readonly IUsersService usersService;
		private readonly IMapper mapper;

		public UsersApiController(
			IUsersService usersService,
			IMapper mapper)
		{
			this.usersService = usersService;
			this.mapper = mapper;
		}

		[HttpGet("{page}")]
		public async Task<ActionResult<AllUsersViewModel>> AllUsers(int page)
		{
			var model = new AllUsersViewModel();
			AllUsersDTO usersDTO = await usersService
				.GetAllUsers(page, model.Pagination.ElementsPerPage);
			model.Users = usersDTO.Users.Select(u => mapper.Map<UserViewModel>(u));
			model.Pagination.Page = usersDTO.Page;
			model.Pagination.TotalElements = usersDTO.AllUsersCount;

			return Ok(model);
		}
	}
}
