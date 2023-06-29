namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.Models.User;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[Area("Admin")]
    [Authorize(Roles = AdminRoleName)]
    public class UsersController : Controller
    {
        private readonly IUsersService userService;

        public UsersController(IUsersService userService)
            => this.userService = userService;

        public async Task<IActionResult> Index(int page = 1)
        {
            UsersInfoDTO usersData = await this.userService.GetUsersInfoAsync(page);
            var viewModel = new UsersViewModel(usersData, page);

            return this.View(viewModel);
        }

        public async Task<IActionResult> Details([Required] Guid id)
        {
            if (!this.ModelState.IsValid)
                return this.BadRequest();

            try
            {
                return this.View(await this.userService.UserDetailsAsync(id));
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }
    }
}
