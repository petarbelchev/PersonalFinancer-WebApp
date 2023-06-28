namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
{
	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.RazorPages;
	using PersonalFinancer.Data.Models;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Data.Constants;

	public class LoginModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly ILogger<LoginModel> logger;

		public LoginModel(
			SignInManager<ApplicationUser> signInManager,
			ILogger<LoginModel> logger)
		{
			this.signInManager = signInManager;
			this.logger = logger;
		}

		[BindProperty]
		public InputModel Input { get; set; } = null!;

		public string ReturnUrl { get; set; } = null!;

		[TempData]
		public string ErrorMessage { get; set; } = null!;

		public class InputModel
		{
			[Required(ErrorMessage = "Email address is required.")]
			[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
			[Display(Name = "Email")]
			public string Email { get; set; } = null!;

			[Required(ErrorMessage = "Password is required.")]
			[DataType(DataType.Password)]
			[StringLength(UserConstants.UserPasswordMaxLength, MinimumLength = UserConstants.UserPasswordMinLength,
				ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
			public string Password { get; set; } = null!;

			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}

		public async Task OnGetAsync(string? returnUrl = null)
		{
			if (!string.IsNullOrEmpty(this.ErrorMessage))
				this.ModelState.AddModelError(string.Empty, this.ErrorMessage);

			returnUrl ??= this.Url.Content("~/");

			await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			this.ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
		{
			returnUrl ??= this.Url.Content("~/");

			if (this.ModelState.IsValid)
			{
				ApplicationUser user = await this.signInManager.UserManager.FindByEmailAsync(this.Input.Email);

				if (user != null)
				{
					var result = await this.signInManager.PasswordSignInAsync(
						user, this.Input.Password, this.Input.RememberMe, lockoutOnFailure: false);

					if (result.Succeeded)
					{
						this.logger.LogInformation("User logged in.");
						return this.LocalRedirect(returnUrl);
					}

					if (result.RequiresTwoFactor)
						return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, this.Input.RememberMe });

					if (result.IsLockedOut)
					{
						this.logger.LogWarning("User account locked out.");
						return this.RedirectToPage("./Lockout");
					}
				}

				this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			}

			return this.Page();
		}
	}
}
