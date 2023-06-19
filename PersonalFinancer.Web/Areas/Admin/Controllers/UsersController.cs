﻿namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.Models.User;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Data.Constants.RoleConstants;

	[Area("Admin")]
    [Authorize(Roles = AdminRoleName)]
    public class UsersController : Controller
    {
        private readonly IUsersService userService;

        public UsersController(IUsersService userService)
            => this.userService = userService;

        public async Task<IActionResult> Index(int page = 1)
        {
            UsersServiceModel usersData = await this.userService.GetAllUsersAsync(page);
            var viewModel = new UsersViewModel { Users = usersData.Users };
            viewModel.Pagination.TotalElements = usersData.TotalUsersCount;
            viewModel.Pagination.Page = page;

            return this.View(viewModel);
        }

        public async Task<IActionResult> Details([Required] Guid? id)
        {
            if (!this.ModelState.IsValid)
                return this.BadRequest();

            try
            {
                Guid userId = id ?? throw new InvalidOperationException();

                return this.View(await this.userService.UserDetailsAsync(userId));
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }
    }
}
