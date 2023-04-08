namespace PersonalFinancer.Web.Models.Shared
{
	using Microsoft.AspNetCore.Mvc;

	using System.ComponentModel.DataAnnotations;

	using Web.ModelBinders;

	public class DateFilterInputModel : IValidatableObject
	{
		[Required(ErrorMessage = "Please enter a valid date.")]
		[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
		[Display(Name = "From")]
		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "Please enter a valid date.")]
		[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
		[Display(Name = "To")]
		public DateTime EndDate { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (StartDate > EndDate)
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
