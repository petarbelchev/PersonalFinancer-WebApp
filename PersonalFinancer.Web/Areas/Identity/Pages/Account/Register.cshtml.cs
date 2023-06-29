namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Text.Encodings.Web;
    using static PersonalFinancer.Common.Constants.UserConstants;
    using static PersonalFinancer.Common.Constants.RoleConstants;

	public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [StringLength(UserNameMaxLength, 
                MinimumLength = UserNameMinLength,
                ErrorMessage = ValidationMessages.InvalidLength)]
            [Display(Name = "Username")]
            public string UserName { get; set; } = null!;

            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [StringLength(UserFirstNameMaxLength, 
                MinimumLength = UserFirstNameMinLength,
                ErrorMessage = ValidationMessages.InvalidLength)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = null!;

            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [StringLength(UserLastNameMaxLength, 
                MinimumLength = UserLastNameMinLength,
                ErrorMessage = ValidationMessages.InvalidLength)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = null!;

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

            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^\d{10}$", ErrorMessage = ValidationMessages.InvalidPhoneNumberLength)]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }
        }

        public void OnGetAsync(string? returnUrl = null) => this.ReturnUrl = returnUrl;

        public async Task<IActionResult> OnPostAsync(string? returnUrl)
        {
            returnUrl ??= this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = this.Input.UserName,
                    FirstName = this.Input.FirstName,
                    LastName = this.Input.LastName,
                    Email = this.Input.Email,
                    PhoneNumber = this.Input.PhoneNumber
                };

                IdentityResult creationResult = await this.userManager.CreateAsync(user, this.Input.Password);

                if (creationResult.Succeeded)
                {
                    await this.userManager.AddToRoleAsync(user, UserRoleName);
                    this.logger.LogInformation("User created a new account with password.");

                    if (this.userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        string code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        string? callbackUrl = this.Url.Page(
                          "/Account/ConfirmEmail",
                          pageHandler: null,
                          values: new { area = "Identity", userId = user.Id, code, returnUrl },
                          protocol: this.Request.Scheme);

                        await this.emailSender.SendEmailAsync(this.Input.Email, "Confirm your email",
                          $"Thank you for your registration, {this.Input.FirstName}! Let's your finances improving begin! " +
                          $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

                        return this.RedirectToPage("RegisterConfirmation");
                    }
                    else
                    {
                        await this.signInManager.SignInAsync(user, isPersistent: false);
                        return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (IdentityError? error in creationResult.Errors)
                    this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.Page();
        }
    }
}
