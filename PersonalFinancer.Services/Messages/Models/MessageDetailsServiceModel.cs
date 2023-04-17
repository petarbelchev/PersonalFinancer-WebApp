namespace PersonalFinancer.Services.Messages.Models
{
	public class MessageDetailsServiceModel
	{
        public string Id { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string Subject { get; set; } = null!;

		public string AuthorName { get; set; } = null!;

		public string Content { get; set; } = null!;

		public IEnumerable<ReplyOutputServiceModel> Replies { get; set; } = null!;
	}
}
