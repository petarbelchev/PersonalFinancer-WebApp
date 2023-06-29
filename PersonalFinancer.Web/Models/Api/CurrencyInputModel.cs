namespace PersonalFinancer.Web.Models.Api
{
    using PersonalFinancer.Common.Messages;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.CurrencyConstants;

    public class CurrencyInputModel : IApiEntityInputModel
    {
        [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
        [StringLength(CurrencyNameMaxLength, MinimumLength = CurrencyNameMinLength,
            ErrorMessage = ValidationMessages.InvalidLength)]
		[Display(Name = "Currency")]
		public string Name { get; set; } = null!;

        [Required]
        public Guid? OwnerId { get; set; }
    }
}
