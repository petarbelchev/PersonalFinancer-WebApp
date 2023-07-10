namespace PersonalFinancer.Web.Models.Shared
{
    using PersonalFinancer.Common.Messages;
    using System.ComponentModel.DataAnnotations;

    public class DateFilterModel : IValidatableObject
    {
        [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
        [Display(Name = "From")]
        public DateTime? FromLocalTime { get; set; }

        [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
        [Display(Name = "To")]
        public DateTime? ToLocalTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.FromLocalTime > this.ToLocalTime)
            {
                yield return new ValidationResult(
                    "Start Date must be before End Date.",
                    new[] { "FromLocalTime" });

                yield return new ValidationResult(
                    "End Date must be after Start Date.",
                    new[] { "ToLocalTime" });
            }
        }
    }
}
