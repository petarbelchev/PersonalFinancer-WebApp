using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Data.Models;
using PersonalFinancer.Web.Models.Account;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		public UserController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public IActionResult Register() => View(new RegisterFormModel());

		[HttpPost]
		public async Task<IActionResult> Register(RegisterFormModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			ApplicationUser newUser = new ApplicationUser
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				UserName = model.Email
			};

			IdentityResult creationResult = await userManager.CreateAsync(newUser, model.Password);

			if (!creationResult.Succeeded)
			{
				foreach (IdentityError error in creationResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}

				return View(model);
			}

			IdentityResult roleResult = await userManager.AddToRoleAsync(newUser, UserRoleName);

			if (!roleResult.Succeeded)
			{
				await userManager.DeleteAsync(newUser);

				foreach (IdentityError error in roleResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}

				return View(model);
			}

			await signInManager.SignInAsync(newUser, isPersistent: false);

			TempData["successMsg"] = "Congratulations! Your registration was successful!";

			return RedirectToAction("Index", "Home");
		}

		public IActionResult Login() => View(new LoginFormModel());
		
		[HttpPost]
		public async Task<IActionResult> Login(LoginFormModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await signInManager.PasswordSignInAsync(
												model.Email,
												model.Password,
												model.RememberMe,
												lockoutOnFailure: false);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError(string.Empty, "Invalid login attempt.");

			return View(model);
		}

		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}
	}
}
