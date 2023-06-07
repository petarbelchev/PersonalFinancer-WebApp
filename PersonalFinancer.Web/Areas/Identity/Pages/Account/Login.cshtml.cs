﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinancer.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
{
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
			[Required]
			[EmailAddress]
			public string Email { get; set; } = null!;

			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; } = null!;

			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}

		public async Task OnGetAsync(string? returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
				ModelState.AddModelError(string.Empty, ErrorMessage);

			returnUrl ??= Url.Content("~/");

			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			if (ModelState.IsValid)
			{
				var result = await signInManager.PasswordSignInAsync(
					Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					logger.LogInformation("User logged in.");
					return LocalRedirect(returnUrl);
				}

				if (result.RequiresTwoFactor)
					return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });

				if (result.IsLockedOut)
				{
					logger.LogWarning("User account locked out.");
					return RedirectToPage("./Lockout");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return Page();
				}
			}

			return Page();
		}
	}
}
