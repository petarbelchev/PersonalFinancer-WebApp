namespace PersonalFinancer.Web.Areas.Identity.Pages.User
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.RazorPages;

	using PersonalFinancer.Data.Models;

	public class LogoutModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly ILogger<LogoutModel> logger;

		public LogoutModel(
			SignInManager<ApplicationUser> signInManager,
			ILogger<LogoutModel> logger)
		{
			this.signInManager = signInManager;
			this.logger = logger;
		}

		public async Task<IActionResult> OnPost()
		{
			await signInManager.SignOutAsync();
			logger.LogInformation("User logged out.");

			return RedirectToPage();
		}
	}
}
