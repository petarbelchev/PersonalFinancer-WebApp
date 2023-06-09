namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PersonalFinancer.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants;

    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<ChangePasswordModel> logger;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        [TempData]
        public string StatusMessage { get; set; } = null!;

        public class InputModel
        {
            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            [StringLength(UserConstants.UserPasswordMaxLength, MinimumLength = UserConstants.UserPasswordMinLength,
                ErrorMessage = "Password must be between {2} and {1} characters long.")]
            [Display(Name = "Old Password")]
            public string OldPassword { get; set; } = null!;

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            [StringLength(UserConstants.UserPasswordMaxLength, MinimumLength = UserConstants.UserPasswordMinLength,
                ErrorMessage = "Password must be between {2} and {1} characters long.")]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; } = null!;

            [Required(ErrorMessage = "Confirm Password is required.")]
            [DataType(DataType.Password)]
            [Compare(nameof(NewPassword), ErrorMessage = "Password do not match.")]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; } = null!;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            
            if (user == null)
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");

            bool hasPassword = await this.userManager.HasPasswordAsync(user);
            return !hasPassword ? this.RedirectToPage("./SetPassword") : this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
                return this.Page();

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            
            if (user == null)
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");

            IdentityResult changePasswordResult = await this.userManager.ChangePasswordAsync(user, this.Input.OldPassword, this.Input.NewPassword);
            
            if (!changePasswordResult.Succeeded)
            {
                foreach (IdentityError? error in changePasswordResult.Errors)
                    this.ModelState.AddModelError(string.Empty, error.Description);

                return this.Page();
            }

            await this.signInManager.RefreshSignInAsync(user);
            this.logger.LogInformation("User changed their password successfully.");
            this.StatusMessage = "Your password has been changed.";

            return this.RedirectToPage();
        }
    }
}
