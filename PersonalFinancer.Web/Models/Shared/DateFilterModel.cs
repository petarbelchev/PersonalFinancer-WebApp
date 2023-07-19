namespace PersonalFinancer.Web.Models.Shared
{
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Web.CustomModelBinders;
	using System.ComponentModel.DataAnnotations;

	public class DateFilterModel : IValidatableObject
    {
        [Required]
        [Display(Name = "From")]
        [ModelBinder(BinderType = typeof(DateTimeModelBinder))]
        public DateTime FromLocalTime { get; set; }

        [Required]
        [Display(Name = "To")]
		[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
		public DateTime ToLocalTime { get; set; }

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
