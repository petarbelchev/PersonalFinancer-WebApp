namespace PersonalFinancer.Web.Models.Message
{
    using PersonalFinancer.Services.Messages.Models;
    using System.ComponentModel.DataAnnotations;

    public class MessageDetailsViewModel
	{
        public string Id { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

		public string Subject { get; set; } = null!;

		public string AuthorName { get; set; } = null!;

		public string Content { get; set; } = null!;

		public IEnumerable<ReplyOutputServiceModel> Replies { get; set; }
			= new List<ReplyOutputServiceModel>();

		[Required(ErrorMessage = "Please enter a message.")]
		[StringLength(1000, MinimumLength = 10,
			ErrorMessage = "Reply must be between {2} and {1} characters long.")]
		public string ReplyContent { get; set; } = null!;
	}
}
