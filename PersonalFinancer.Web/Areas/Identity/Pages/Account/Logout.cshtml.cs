using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
{
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

		public async Task<IActionResult> OnPost(string? returnUrl = null)
		{
			await signInManager.SignOutAsync();
			logger.LogInformation("User logged out.");

			if (returnUrl != null)
				return LocalRedirect(returnUrl);
			else
				return RedirectToPage();
		}
	}
}
