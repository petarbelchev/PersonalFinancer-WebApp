namespace PersonalFinancer.Web.Models.Message
{
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.MessageConstants;

	public class MessageDetailsViewModel
	{
        public string Id { get; set; } = null!;

        public DateTime CreatedOnUtc { get; set; }

		public string Subject { get; set; } = null!;

		public string AuthorId { get; set; } = null!;

        public string AuthorName { get; set; } = null!;

		public string Content { get; set; } = null!;

		public IEnumerable<ReplyOutputDTO> Replies { get; set; }
			= new List<ReplyOutputDTO>();

		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(ReplyMaxLength, 
			MinimumLength = ReplyMinLength,
			ErrorMessage = ValidationMessages.InvalidLength)]
		[Display(Name = "Reply content")]
		public string ReplyContent { get; set; } = null!;

		public string? ImageToBase64String { get; set; }
	}
}
