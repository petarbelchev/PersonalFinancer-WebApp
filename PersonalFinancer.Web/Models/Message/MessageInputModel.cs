namespace PersonalFinancer.Web.Models.Message
{
    using System.ComponentModel.DataAnnotations;

    public class MessageInputModel
	{
		[Required(ErrorMessage = "Please enter a subject.")]
		[StringLength(50, MinimumLength = 10, 
			ErrorMessage = "Subject must be between {2} and {1} characters long.")]
		public string Subject { get; set; } = null!;
		
		[Required(ErrorMessage = "Please enter a message.")]
		[StringLength(1000, MinimumLength = 20, 
			ErrorMessage = "Message must be between {2} and {1} characters long.")]
		public string Content { get; set; } = null!;
    }
}
