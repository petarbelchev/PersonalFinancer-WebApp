using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using PersonalFinancer.Data.Models;
using System.Text;

namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
{
	public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
            => this.userManager = userManager;

        public bool IsConfirmed { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
                return RedirectToPage("/Index");

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userId}'.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ConfirmEmailAsync(user, code);

            IsConfirmed = result.Succeeded;

            return Page();
        }
    }
}
