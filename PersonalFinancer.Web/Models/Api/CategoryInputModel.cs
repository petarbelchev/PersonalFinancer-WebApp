namespace PersonalFinancer.Web.Models.Api
{
    using PersonalFinancer.Common.Messages;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Common.Constants.CategoryConstants;

    public class CategoryInputModel : IApiEntityInputModel
    {
        [Required(ErrorMessage = ValidationMessages.RequiredProperty)]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength,
            ErrorMessage = ValidationMessages.InvalidLength)]
		[Display(Name = "Category")]
		public string Name { get; set; } = null!;

        [Required]
        public Guid? OwnerId { get; set; }
    }
}
