namespace PersonalFinancer.Web.Controllers
{
	using Data.Models;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Models.Account;

	/// <summary>
	/// User Controller takes care of everything related to Users.
	/// </summary>
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

		/// <summary>
		/// Returns Register Form Model for Register page.
		/// </summary>
		[HttpGet]
		public IActionResult Register()
		{
			RegisterFormModel model = new RegisterFormModel();

			return View(model);
		}

		/// <summary>
		/// Handle with Register Form Model and register a new User.
		/// </summary>
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

			IdentityResult result = await userManager.CreateAsync(newUser, model.Password);

			if (result.Succeeded)
			{
				await signInManager.SignInAsync(newUser, isPersistent: false);

				TempData["successMsg"] = "Congratulations! Your registration was successful!";

				return RedirectToAction("Index", "Home");
			}

			foreach (IdentityError error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model);
		}

		/// <summary>
		/// Returns Login Form Model for Login page.
		/// </summary>
		[HttpGet]
		public IActionResult Login()
		{
			LoginFormModel loginFormModel = new LoginFormModel();

			return View(loginFormModel);
		}

		/// <summary>
		/// Handle with Login Form Model and login a User.
		/// </summary>
		public async Task<IActionResult> Login(LoginFormModel loginFormModel)
		{
			if (!ModelState.IsValid)
			{
				return View(loginFormModel);
			}

			var result = await signInManager.PasswordSignInAsync(
				loginFormModel.Email,
				loginFormModel.Password,
				loginFormModel.RememberMe, false);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError(string.Empty, "Invalid login attempt.");

			return View(loginFormModel);
		}

		/// <summary>
		/// Logout a User.
		/// </summary>
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}
	}
}
