#nullable disable
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
	public class TwoFactorAuthenticationModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		public TwoFactorAuthenticationModel(
			UserManager<ApplicationUser> userManager, 
			SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public bool HasAuthenticator { get; set; }

		public int RecoveryCodesLeft { get; set; }

		[BindProperty]
		public bool Is2faEnabled { get; set; }

		public bool IsMachineRemembered { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await userManager.GetUserAsync(User);

			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null;
			Is2faEnabled = await userManager.GetTwoFactorEnabledAsync(user);
			IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user);
			RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var user = await userManager.GetUserAsync(User);

			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			await signInManager.ForgetTwoFactorClientAsync();
			StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
			
			return RedirectToPage();
		}
	}
}
