namespace PersonalFinancer.Web.Models.Api
{
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.CategoryConstants;

    public class CategoryInputModel : IApiInputModel
    {
        [Required(ErrorMessage = "Please enter a Category name.")]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength,
            ErrorMessage = "Category name must be between {2} and {1} characters long.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Owner Id is required!")]
        public Guid? OwnerId { get; set; }
    }
}
