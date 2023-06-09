namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
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

        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            await this.signInManager.SignOutAsync();
            this.logger.LogInformation("User logged out.");

            return returnUrl != null 
                ? this.LocalRedirect(returnUrl) 
                : this.RedirectToPage();
        }
    }
}
