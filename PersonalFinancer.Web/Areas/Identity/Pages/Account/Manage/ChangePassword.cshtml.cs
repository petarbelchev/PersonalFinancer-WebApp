namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PersonalFinancer.Common.Messages;
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
            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [DataType(DataType.Password)]
            [StringLength(UserConstants.UserPasswordMaxLength, 
                MinimumLength = UserConstants.UserPasswordMinLength,
                ErrorMessage = ValidationMessages.InvalidLength)]
            [Display(Name = "Old Password")]
            public string OldPassword { get; set; } = null!;

            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [DataType(DataType.Password)]
            [StringLength(UserConstants.UserPasswordMaxLength, 
                MinimumLength = UserConstants.UserPasswordMinLength,
                ErrorMessage = ValidationMessages.InvalidLength)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; } = null!;

            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [DataType(DataType.Password)]
            [Compare(nameof(NewPassword), ErrorMessage = ValidationMessages.CompareDoNotMatch)]
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
