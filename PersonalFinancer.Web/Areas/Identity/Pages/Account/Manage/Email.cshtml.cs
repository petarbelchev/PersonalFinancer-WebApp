namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using PersonalFinancer.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Text.Encodings.Web;

    public class EmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;

        public EmailModel(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        public string Email { get; set; } = null!;

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; } = null!;

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public class InputModel
        {
            [Required(ErrorMessage = "Email address is required.")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            [Display(Name = "New Email")]
            public string NewEmail { get; set; } = null!;
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            this.Email = user.Email;

            this.IsEmailConfirmed = await this.userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");

            await this.LoadAsync(user);

            return this.Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");

            if (!this.ModelState.IsValid)
            {
                await this.LoadAsync(user);
                return this.Page();
            }

            if (this.Input.NewEmail != user.Email)
            {
                string userId = await this.userManager.GetUserIdAsync(user);
                string code = await this.userManager.GenerateChangeEmailTokenAsync(user, this.Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                string? callbackUrl = this.Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId, email = this.Input.NewEmail, code },
                    protocol: this.Request.Scheme);

                await this.emailSender.SendEmailAsync(
                    this.Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

                this.StatusMessage = "Confirmation link to change email sent. Please check your email.";
                
                return this.RedirectToPage();
            }

            this.StatusMessage = "Your email is unchanged.";

            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");

            if (!this.ModelState.IsValid)
            {
                await this.LoadAsync(user);
                return this.Page();
            }

            string userId = await this.userManager.GetUserIdAsync(user);
            string email = await this.userManager.GetEmailAsync(user);
            string code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            
            string? callbackUrl = this.Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code },
                protocol: this.Request.Scheme);
            
            await this.emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

            this.StatusMessage = "Verification email sent. Please check your email.";

            return this.RedirectToPage();
        }
    }
}
