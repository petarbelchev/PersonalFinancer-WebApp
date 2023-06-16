namespace PersonalFinancer.Web.Models.Api
{
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.CurrencyConstants;

    public class CurrencyInputModel : IApiInputModel
    {
        [Required(ErrorMessage = "Please enter a Currency name.")]
        [StringLength(CurrencyNameMaxLength, MinimumLength = CurrencyNameMinLength,
            ErrorMessage = "Currency name must be between {2} and {1} characters long.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Owner Id is required!")]
        public Guid? OwnerId { get; set; }
    }
}
