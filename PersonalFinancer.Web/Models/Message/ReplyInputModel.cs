namespace PersonalFinancer.Web.Models.Message
{
    using System.ComponentModel.DataAnnotations;

    public class ReplyInputModel
	{
		[Required]
		public string Id { get; set; } = null!;

		[Required(ErrorMessage = "Plaese enter a message.")]
		[StringLength(1000, MinimumLength = 10, 
			ErrorMessage = "Reply must be between {2} and {1} characters long.")]
        public string ReplyContent { get; set; } = null!;
    }
}
