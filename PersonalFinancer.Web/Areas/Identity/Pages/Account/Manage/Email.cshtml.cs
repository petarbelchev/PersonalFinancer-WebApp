using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using PersonalFinancer.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
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
			Email = user.Email;

			IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await userManager.GetUserAsync(User);

			if (user == null)
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

			await LoadAsync(user);

			return Page();
		}

		public async Task<IActionResult> OnPostChangeEmailAsync()
		{
			var user = await userManager.GetUserAsync(User);

			if (user == null)
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			if (Input.NewEmail != user.Email)
			{
				var userId = await userManager.GetUserIdAsync(user);
				var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page(
					"/Account/ConfirmEmailChange",
					pageHandler: null,
					values: new { area = "Identity", userId, email = Input.NewEmail, code },
					protocol: Request.Scheme);
				await emailSender.SendEmailAsync(
					Input.NewEmail,
					"Confirm your email",
					$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

				StatusMessage = "Confirmation link to change email sent. Please check your email.";
				return RedirectToPage();
			}

			StatusMessage = "Your email is unchanged.";
			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostSendVerificationEmailAsync()
		{
			var user = await userManager.GetUserAsync(User);

			if (user == null)
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			var userId = await userManager.GetUserIdAsync(user);
			var email = await userManager.GetEmailAsync(user);
			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			var callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				pageHandler: null,
				values: new { area = "Identity", userId, code },
				protocol: Request.Scheme);
			await emailSender.SendEmailAsync(
				email,
				"Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

			StatusMessage = "Verification email sent. Please check your email.";
			return RedirectToPage();
		}
	}
}
