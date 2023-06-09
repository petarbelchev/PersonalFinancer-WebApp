namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using PersonalFinancer.Data.Models;
    using System.Text;

    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
            => this.userManager = userManager;

        public bool IsConfirmed { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
                return this.RedirectToPage("/Index");

            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
                return this.NotFound($"Unable to load user with ID '{userId}'.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            IdentityResult result = await this.userManager.ConfirmEmailAsync(user, code);

            this.IsConfirmed = result.Succeeded;

            return this.Page();
        }
    }
}
