namespace PersonalFinancer.Web.Models.Api
{
    using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Web.CustomAttributes;
	using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Common.Constants.AccountTypeConstants;

    public class AccountTypeInputModel : IApiEntityInputModel
    {
        [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
        [StringLength(AccountTypeNameMaxLength, MinimumLength = AccountTypeNameMinLength,
            ErrorMessage = ValidationMessages.InvalidLength)]
        [Display(Name = "Account Type")]
		[RequireHtmlEncoding]
		public string Name { get; set; } = null!;

        [Required]
        public Guid? OwnerId { get; set; }
    }
}
