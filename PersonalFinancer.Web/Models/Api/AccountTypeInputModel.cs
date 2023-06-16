namespace PersonalFinancer.Web.Models.Api
{
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.AccountTypeConstants;

    public class AccountTypeInputModel : IApiInputModel
    {
        [Required(ErrorMessage = "Please enter an Account Type name.")]
        [StringLength(AccountTypeNameMaxLength, MinimumLength = AccountTypeNameMinLength,
            ErrorMessage = "Account Type name must be between {2} and {1} characters long.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Owner Id is required!")]
        public Guid? OwnerId { get; set; }
    }
}
