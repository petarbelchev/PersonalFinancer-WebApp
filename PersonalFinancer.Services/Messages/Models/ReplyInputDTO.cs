namespace PersonalFinancer.Services.Messages.Models
{
	public class ReplyInputDTO
	{
        public string MessageId { get; set; } = null!;

        public string AuthorId { get; set; } = null!;

        public bool IsAuthorAdmin { get; set; }

        public string AuthorName { get; set; } = null!;

        public string Content { get; set; } = null!;
    }
}
