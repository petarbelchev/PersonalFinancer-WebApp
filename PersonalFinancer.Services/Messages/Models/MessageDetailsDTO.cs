namespace PersonalFinancer.Services.Messages.Models
{
	public class MessageDetailsDTO
	{
		public string Id { get; set; } = null!;

        public DateTime CreatedOnUtc { get; set; }

		public string AuthorId { get; set; } = null!;

		public string AuthorName { get; set; } = null!;

		public string Subject { get; set; } = null!;

		public string Content { get; set; } = null!;

		public byte[]? Image { get; set; }

		public IEnumerable<ReplyOutputDTO> Replies { get; set; } = null!;
	}
}
