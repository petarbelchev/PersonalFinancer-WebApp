using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using PersonalFinancer.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using static PersonalFinancer.Data.Constants;
using static PersonalFinancer.Web.Infrastructure.Constants;

namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
{
	public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly IUserEmailStore<ApplicationUser> emailStore;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.userStore = userStore;
            this.emailStore = GetEmailStore();
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "First Name is required.")]
            [StringLength(UserConstants.UserFirstNameMaxLength, MinimumLength = UserConstants.UserFirstNameMinLength,
                ErrorMessage = "First name must be between {2} and {1} characters long.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = null!;

            [Required(ErrorMessage = "Last Name is required.")]
            [StringLength(UserConstants.UserLastNameMaxLength, MinimumLength = UserConstants.UserLastNameMinLength,
                ErrorMessage = "Last name must be between {2} and {1} characters long.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = null!;

            [Required(ErrorMessage = "Email address is required.")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = null!;

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            [StringLength(UserConstants.UserPasswordMaxLength, MinimumLength = UserConstants.UserPasswordMinLength,
                ErrorMessage = "Password must be between {2} and {1} characters long.")]
            public string Password { get; set; } = null!;

            [Required(ErrorMessage = "Confirm Password is required.")]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "Password do not match.")]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; } = null!;

            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }
        }


        public void OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber
                };

                await userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var creationResult = await userManager.CreateAsync(user, Input.Password);

                if (creationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, RoleConstants.UserRoleName);
                    logger.LogInformation("User created a new account with password.");

                    if (userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code, returnUrl },
                            protocol: Request.Scheme);

                        await emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Thank you for your registration, {Input.FirstName}! Let's your finances improving begin! " +
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

                        return RedirectToPage("RegisterConfirmation");
                    }
                    else
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in creationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)userStore;
        }
    }
}
