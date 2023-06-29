namespace PersonalFinancer.Web.Areas.Identity.Pages.Account.Manage
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants;

    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; } = null!;

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public class InputModel
        {
            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [StringLength(UserConstants.UserNameMaxLength, 
                MinimumLength = UserConstants.UserNameMinLength,
                ErrorMessage = ValidationMessages.InvalidLength)]
            [Display(Name = "Username")]
            public string UserName { get; set; } = null!;

            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [StringLength(UserConstants.UserFirstNameMaxLength, 
                MinimumLength = UserConstants.UserFirstNameMinLength,
                ErrorMessage = ValidationMessages.InvalidLength)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = null!;

            [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
            [StringLength(UserConstants.UserLastNameMaxLength, 
                MinimumLength = UserConstants.UserLastNameMinLength,
                ErrorMessage = ValidationMessages.InvalidLength)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = null!;

            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^\d{10}$", ErrorMessage = ValidationMessages.InvalidPhoneNumberLength)]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");

            this.Input = new InputModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");

            if (!this.ModelState.IsValid)
                return this.Page();

            user.UserName = this.Input.UserName;
            user.FirstName = this.Input.FirstName;
            user.LastName = this.Input.LastName;
            user.PhoneNumber = this.Input.PhoneNumber;

            await this.userManager.UpdateAsync(user);

            await this.signInManager.RefreshSignInAsync(user);
            this.StatusMessage = "Your profile has been updated";

            return this.RedirectToPage();
        }
    }
}
