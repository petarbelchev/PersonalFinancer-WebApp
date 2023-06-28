namespace PersonalFinancer.Web.Models.Shared
{
	using System.ComponentModel.DataAnnotations;

	public class DateFilterModel : IValidatableObject
    {
        [Required(ErrorMessage = "Please enter a valid date.")]
        [Display(Name = "From")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Please enter a valid date.")]
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
