namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;

	using Models.Account;
	using Data.Models;

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

		[HttpGet]
		public IActionResult Register(string? returnUrl = null)
		{
			var model = new RegisterFormModel();

			if (returnUrl != null)
				model.ReturnUrl = returnUrl;

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterFormModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var newUser = new ApplicationUser
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				UserName = model.Email
			};

			var result = await userManager.CreateAsync(newUser, model.Password);

			if (result.Succeeded)
			{
				await signInManager.SignInAsync(newUser, isPersistent: false);

				if (model.ReturnUrl != null)
					return LocalRedirect(model.ReturnUrl);
				else
					return RedirectToAction("Index", "Home");
			}

			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error.Description);

			return View(model);
		}

		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			var loginFormModel = new LoginFormModel();

			if (returnUrl != null)
				loginFormModel.ReturnUrl = returnUrl;

			return View(loginFormModel);
		}

		public async Task<IActionResult> Login(LoginFormModel loginFormModel)
		{
			if (!ModelState.IsValid)
				return View(loginFormModel);

			var result = await signInManager.PasswordSignInAsync(
				loginFormModel.Email,
				loginFormModel.Password,
				loginFormModel.RememberMe, false);

			if (result.Succeeded)
			{
				if (loginFormModel.ReturnUrl != null)
					return LocalRedirect(loginFormModel.ReturnUrl);
				else
					return RedirectToAction("Index", "Dashboard");
			}

			ModelState.AddModelError(string.Empty, "Invalid login attempt.");

			return View(loginFormModel);
		}

		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}
	}
}
