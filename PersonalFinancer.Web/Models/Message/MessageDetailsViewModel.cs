namespace PersonalFinancer.Web.Models.Message
{
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Services.Messages.Models;
    using System.ComponentModel.DataAnnotations;

    public class MessageDetailsViewModel
	{
        public string Id { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

		public string Subject { get; set; } = null!;

		public string AuthorId { get; set; } = null!;

        public string AuthorName { get; set; } = null!;

		public string Content { get; set; } = null!;

		public IEnumerable<ReplyOutputDTO> Replies { get; set; }
			= new List<ReplyOutputDTO>();

		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(1000, MinimumLength = 10,
			ErrorMessage = ValidationMessages.InvalidLength)]
		public string ReplyContent { get; set; } = null!;
	}
}
