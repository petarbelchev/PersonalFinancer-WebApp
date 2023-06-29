namespace PersonalFinancer.Web.Models.Shared
{
    using PersonalFinancer.Common.Messages;
    using System.ComponentModel.DataAnnotations;

    public class DateFilterModel : IValidatableObject
    {
        [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
        [Display(Name = "From")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
        [Display(Name = "To")]
        public DateTime? EndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.StartDate > this.EndDate)
            {
                yield return new ValidationResult(
                    "Start Date must be before End Date.",
                    new[] { "StartDate" });

                yield return new ValidationResult(
                    "End Date must be after Start Date.",
                    new[] { "EndDate" });
            }
        }
    }
}
