﻿#nullable disable
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinancer.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
	public class EnableAuthenticatorModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<EnableAuthenticatorModel> logger;
		private readonly UrlEncoder urlEncoder;

		private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

		public EnableAuthenticatorModel(
			UserManager<ApplicationUser> userManager,
			ILogger<EnableAuthenticatorModel> logger,
			UrlEncoder urlEncoder)
		{
			this.userManager = userManager;
			this.logger = logger;
			this.urlEncoder = urlEncoder;
		}

		public string SharedKey { get; set; }

		public string AuthenticatorUri { get; set; }

		[TempData]
		public string[] RecoveryCodes { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required]
			[StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Text)]
			[Display(Name = "Verification Code")]
			public string Code { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			await LoadSharedKeyAndQrCodeUriAsync(user);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid)
			{
				await LoadSharedKeyAndQrCodeUriAsync(user);
				return Page();
			}

			// Strip spaces and hyphens
			var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

			var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
				user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

			if (!is2faTokenValid)
			{
				ModelState.AddModelError("Input.Code", "Verification code is invalid.");
				await LoadSharedKeyAndQrCodeUriAsync(user);
				return Page();
			}

			await userManager.SetTwoFactorEnabledAsync(user, true);
			var userId = await userManager.GetUserIdAsync(user);
			logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

			StatusMessage = "Your authenticator app has been verified.";

			if (await userManager.CountRecoveryCodesAsync(user) == 0)
			{
				var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
				RecoveryCodes = recoveryCodes.ToArray();
				return RedirectToPage("./ShowRecoveryCodes");
			}
			else
			{
				return RedirectToPage("./TwoFactorAuthentication");
			}
		}

		private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
		{
			// Load the authenticator key & QR code URI to display on the form
			var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
			if (string.IsNullOrEmpty(unformattedKey))
			{
				await userManager.ResetAuthenticatorKeyAsync(user);
				unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
			}

			SharedKey = FormatKey(unformattedKey);

			var email = await userManager.GetEmailAsync(user);
			AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
		}

		private string FormatKey(string unformattedKey)
		{
			var result = new StringBuilder();
			int currentPosition = 0;
			while (currentPosition + 4 < unformattedKey.Length)
			{
				result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
				currentPosition += 4;
			}
			if (currentPosition < unformattedKey.Length)
			{
				result.Append(unformattedKey.AsSpan(currentPosition));
			}

			return result.ToString().ToLowerInvariant();
		}

		private string GenerateQrCodeUri(string email, string unformattedKey)
		{
			return string.Format(
				CultureInfo.InvariantCulture,
				AuthenticatorUriFormat,
				urlEncoder.Encode("Personal Financer"),
				urlEncoder.Encode(email),
				unformattedKey);
		}
	}
}
