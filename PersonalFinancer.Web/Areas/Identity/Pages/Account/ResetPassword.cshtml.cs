using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using PersonalFinancer.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Web.Areas.Identity.Pages.Account
{
	public class ResetPasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;

		public ResetPasswordModel(UserManager<ApplicationUser> userManager)
			=> this.userManager = userManager;

		[BindProperty]
		public InputModel Input { get; set; } = null!;

		public class InputModel
		{
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

			[Required]
			public string Code { get; set; } = null!;
		}

		public IActionResult OnGet(string? code = null)
		{
			if (code == null)
			{
				return BadRequest("A code must be supplied for password reset.");
			}
			else
			{
				Input = new InputModel
				{
					Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
				};

				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
				return Page();

			var user = await userManager.FindByEmailAsync(Input.Email);

			if (user == null)
				return RedirectToPage("./ResetPasswordConfirmation");

			var result = await userManager.ResetPasswordAsync(user, Input.Code, Input.Password);

			if (result.Succeeded)
				return RedirectToPage("./ResetPasswordConfirmation");

			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error.Description);

			return Page();
		}
	}
}
