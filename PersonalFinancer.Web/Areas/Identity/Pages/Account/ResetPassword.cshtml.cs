namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using static PersonalFinancer.Common.Constants.UserConstants;

    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
            => this.userManager = userManager;

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public class InputModel
        {
            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [EmailAddress(ErrorMessage = ValidationMessages.InvalidEmailAddress)]
            [Display(Name = "Email")]
            public string Email { get; set; } = null!;

            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [DataType(DataType.Password)]
            [StringLength(UserPasswordMaxLength, 
                MinimumLength = UserPasswordMinLength,
                ErrorMessage = ValidationMessages.InvalidLength)]
            public string Password { get; set; } = null!;

            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = ValidationMessages.CompareDoNotMatch)]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; } = null!;

            [Required]
            public string Code { get; set; } = null!;
        }

        public IActionResult OnGet(string? code = null)
        {
            if (code == null)
            {
                return this.BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                this.Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };

                return this.Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
                return this.Page();

            ApplicationUser user = await this.userManager.FindByEmailAsync(this.Input.Email);

            if (user == null)
                return this.RedirectToPage("./ResetPasswordConfirmation");

            IdentityResult result = await this.userManager.ResetPasswordAsync(user, this.Input.Code, this.Input.Password);

            if (result.Succeeded)
                return this.RedirectToPage("./ResetPasswordConfirmation");

            foreach (IdentityError? error in result.Errors)
                this.ModelState.AddModelError(string.Empty, error.Description);

            return this.Page();
        }
    }
}
