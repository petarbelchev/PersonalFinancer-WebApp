namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Users;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[Area("Admin")]
    [Authorize(Roles = AdminRoleName)]
    public class UsersController : Controller
    {
        private readonly IUsersService userService;
		private readonly ILogger<UsersController> logger;

		public UsersController(
			IUsersService userService,
			ILogger<UsersController> logger)
		{
			this.userService = userService;
			this.logger = logger;
		}

		public IActionResult Index() => this.View();

		public async Task<IActionResult> Details([Required] Guid id)
        {
            if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.GetUserDetailsWithInvalidInputData, 
					this.User.Id());

                return this.BadRequest();
			}

            try
            {
                return this.View(await this.userService.UserDetailsAsync(id));
            }
            catch (InvalidOperationException)
			{
				this.logger.LogWarning(
					LoggerMessages.GetUserDetailsWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
            }
        }
    }
}
