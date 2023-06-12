namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PersonalFinancer.Data.Models;

    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public PersonalDataModel(UserManager<ApplicationUser> userManager) 
            => this.userManager = userManager;

        public async Task<IActionResult> OnGet()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            return user == null 
                ? this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.") 
                : this.Page();
        }
    }
}
