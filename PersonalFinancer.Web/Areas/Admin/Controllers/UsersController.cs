namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.User;
	using Services.User.Models;

	using Web.Models.User;

	using static Data.Constants.RoleConstants;

	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class UsersController : Controller
	{
		private readonly IUsersService userService;
		private readonly IMapper mapper;

		public UsersController(
			IUsersService userService,
			IMapper mapper)
		{
			this.userService = userService;
			this.mapper = mapper;
		}

		public async Task<IActionResult> Index(int page = 1)
		{
			var viewModel = new AllUsersViewModel();
			AllUsersDTO usersDTO = await userService
				.GetAllUsers(page, viewModel.Pagination.ElementsPerPage);

			viewModel.Users = usersDTO.Users
				.Select(u => mapper.Map<UserViewModel>(u)).ToArray();
			viewModel.Pagination.Page = usersDTO.Page;
			viewModel.Pagination.TotalElements = usersDTO.AllUsersCount;

			return View(viewModel);
		}

		public async Task<IActionResult> Details(string id)
		{
			UserDetailsDTO userDetailsDTO = await userService.UserDetails(id);
			var viewModel = mapper.Map<UserDetailsViewModel>(userDetailsDTO);

			return View(viewModel);
		}
	}
}
