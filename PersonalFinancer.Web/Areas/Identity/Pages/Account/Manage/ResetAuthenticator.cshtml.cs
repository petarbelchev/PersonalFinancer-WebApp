namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
#nullable disable
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PersonalFinancer.Data.Models;

    public class ResetAuthenticatorModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<ResetAuthenticatorModel> logger;

        public ResetAuthenticatorModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ResetAuthenticatorModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            return user == null 
                ? this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.") 
                : this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            
            if (user == null)
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");

            _ = await this.userManager.SetTwoFactorEnabledAsync(user, false);
            _ = await this.userManager.ResetAuthenticatorKeyAsync(user);

            this.logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

            await this.signInManager.RefreshSignInAsync(user);
            this.StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

            return this.RedirectToPage("./EnableAuthenticator");
        }
    }
}
