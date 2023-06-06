using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinancer.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
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
			[Required(ErrorMessage = "Password is required.")]
			[DataType(DataType.Password)]
			[StringLength(UserConstants.UserPasswordMaxLength, MinimumLength = UserConstants.UserPasswordMinLength,
				ErrorMessage = "Password must be between {2} and {1} characters long.")]
			[Display(Name = "Old Password")]
			public string OldPassword { get; set; } = null!;

			[Required(ErrorMessage = "Password is required.")]
			[DataType(DataType.Password)]
			[StringLength(UserConstants.UserPasswordMaxLength, MinimumLength = UserConstants.UserPasswordMinLength,
				ErrorMessage = "Password must be between {2} and {1} characters long.")]
			[Display(Name = "New Password")]
			public string NewPassword { get; set; } = null!;

			[Required(ErrorMessage = "Confirm Password is required.")]
			[DataType(DataType.Password)]
			[Compare(nameof(NewPassword), ErrorMessage = "Password do not match.")]
			[Display(Name = "Confirm Password")]
			public string ConfirmPassword { get; set; } = null!;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			var hasPassword = await userManager.HasPasswordAsync(user);
			if (!hasPassword)
			{
				return RedirectToPage("./SetPassword");
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			var changePasswordResult = await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
			if (!changePasswordResult.Succeeded)
			{
				foreach (var error in changePasswordResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return Page();
			}

			await signInManager.RefreshSignInAsync(user);
			logger.LogInformation("User changed their password successfully.");
			StatusMessage = "Your password has been changed.";

			return RedirectToPage();
		}
	}
}
